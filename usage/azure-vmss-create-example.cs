using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Rest;

namespace AzureVmssExample
{
    /// <summary>
    /// Creates an Azure Virtual Machine Scale Set with encrypted data disk using DES
    /// </summary>
    /// <remarks>
    /// Requirements:
    /// - NuGet: Microsoft.Azure.Management.Compute 59.0.0 (from Directory.Packages.props)
    /// - IAM: Contributor role on target resource group
    /// - IAM: User Assigned Identity or Service Principal with permissions to DES resource
    /// - Prerequisites: 
    ///   * Virtual Network and Subnet must exist
    ///   * Disk Encryption Set must exist with appropriate Key Vault permissions
    /// </remarks>
    public class VmssCreator
    {
        private readonly ComputeManagementClient _computeClient;
        private readonly NetworkManagementClient _networkClient;

        public VmssCreator(TokenCredentials credentials, string subscriptionId)
        {
            _computeClient = new ComputeManagementClient(credentials)
            {
                SubscriptionId = subscriptionId
            };

            _networkClient = new NetworkManagementClient(credentials)
            {
                SubscriptionId = subscriptionId
            };
        }

        /// <summary>
        /// Creates a VM Scale Set with Debian marketplace image and encrypted data disk
        /// </summary>
        /// <param name="resourceGroupName">Target resource group name</param>
        /// <param name="location">Azure region (e.g., "eastus")</param>
        /// <param name="vmssName">Name for the VM scale set</param>
        /// <param name="subnetId">Full resource ID of the subnet</param>
        /// <param name="diskEncryptionSetId">Full resource ID of the Disk Encryption Set</param>
        /// <param name="adminUsername">Admin username for VMs</param>
        /// <param name="sshPublicKey">SSH public key for authentication</param>
        /// <returns>Created VirtualMachineScaleSet</returns>
        public async Task<VirtualMachineScaleSet> CreateVmssWithEncryptedDataDiskAsync(
            string resourceGroupName,
            string location,
            string vmssName,
            string subnetId,
            string diskEncryptionSetId,
            string adminUsername,
            string sshPublicKey)
        {
            // Define the Debian marketplace image reference
            // Publisher: Debian, Offer: debian-11, SKU: 11-gen2
            var imageReference = new ImageReference
            {
                Publisher = "Debian",
                Offer = "debian-11",
                Sku = "11-gen2",
                Version = "latest"
            };

            // Configure data disk with DES encryption
            // - 128 GB size
            // - ReadOnly caching for read-heavy workloads
            // - LUN 0 (first data disk)
            // - Standard_LRS storage (can be changed to Premium_LRS)
            var dataDisks = new List<VirtualMachineScaleSetDataDisk>
            {
                new VirtualMachineScaleSetDataDisk
                {
                    Lun = 0,
                    CreateOption = DiskCreateOptionTypes.Empty,
                    DiskSizeGB = 128,
                    Caching = CachingTypes.ReadOnly,
                    ManagedDisk = new VirtualMachineScaleSetManagedDiskParameters
                    {
                        StorageAccountType = StorageAccountTypes.StandardLRS,
                        DiskEncryptionSet = new DiskEncryptionSetParameters
                        {
                            Id = diskEncryptionSetId
                        }
                    }
                }
            };

            // Configure OS disk (also encrypted via DES)
            var osDisk = new VirtualMachineScaleSetOSDisk
            {
                CreateOption = DiskCreateOptionTypes.FromImage,
                Caching = CachingTypes.ReadWrite,
                ManagedDisk = new VirtualMachineScaleSetManagedDiskParameters
                {
                    StorageAccountType = StorageAccountTypes.StandardSSDLRS,
                    DiskEncryptionSet = new DiskEncryptionSetParameters
                    {
                        Id = diskEncryptionSetId
                    }
                }
            };

            // Storage profile with marketplace image, OS disk, and data disk
            var storageProfile = new VirtualMachineScaleSetStorageProfile
            {
                ImageReference = imageReference,
                OsDisk = osDisk,
                DataDisks = dataDisks
            };

            // OS profile for Linux with SSH authentication
            var osProfile = new VirtualMachineScaleSetOSProfile
            {
                ComputerNamePrefix = vmssName.Substring(0, Math.Min(9, vmssName.Length)),
                AdminUsername = adminUsername,
                LinuxConfiguration = new LinuxConfiguration
                {
                    DisablePasswordAuthentication = true,
                    Ssh = new SshConfiguration
                    {
                        PublicKeys = new List<SshPublicKey>
                        {
                            new SshPublicKey
                            {
                                Path = $"/home/{adminUsername}/.ssh/authorized_keys",
                                KeyData = sshPublicKey
                            }
                        }
                    }
                }
            };

            // Network configuration
            var ipConfig = new VirtualMachineScaleSetIPConfiguration
            {
                Name = "ipconfig1",
                Subnet = new ApiEntityReference
                {
                    Id = subnetId
                }
            };

            var nicConfig = new VirtualMachineScaleSetNetworkConfiguration
            {
                Name = "nicconfig1",
                Primary = true,
                IpConfigurations = new List<VirtualMachineScaleSetIPConfiguration> { ipConfig },
                EnableAcceleratedNetworking = false
            };

            var networkProfile = new VirtualMachineScaleSetNetworkProfile
            {
                NetworkInterfaceConfigurations = new List<VirtualMachineScaleSetNetworkConfiguration> { nicConfig }
            };

            // VM scale set parameters
            // Using Standard_D2s_v3 (Standard instance type, 2 vCPUs, 8 GB RAM)
            var vmssParameters = new VirtualMachineScaleSet
            {
                Location = location,
                Sku = new Sku
                {
                    Name = "Standard_D2s_v3",
                    Tier = "Standard",
                    Capacity = 2 // Initial instance count
                },
                UpgradePolicy = new UpgradePolicy
                {
                    Mode = UpgradeMode.Automatic
                },
                VirtualMachineProfile = new VirtualMachineScaleSetVMProfile
                {
                    StorageProfile = storageProfile,
                    OsProfile = osProfile,
                    NetworkProfile = networkProfile
                },
                Overprovision = true
            };

            // Create the VM scale set
            var vmss = await _computeClient.VirtualMachineScaleSets.CreateOrUpdateAsync(
                resourceGroupName,
                vmssName,
                vmssParameters
            );

            return vmss;
        }

        /// <summary>
        /// Example usage demonstrating VMSS creation
        /// </summary>
        public static async Task Main(string[] args)
        {
            // Authentication: Use Azure CLI credentials or Service Principal
            // For production: Use Managed Identity or Azure.Identity.DefaultAzureCredential
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(
                    clientId: Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
                    clientSecret: Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET"),
                    tenantId: Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
                    environment: AzureEnvironment.AzureGlobalCloud
                )
                .WithDefaultSubscription();

            string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");
            string resourceGroupName = "myResourceGroup";
            string location = "eastus";
            string vmssName = "myVmss";
            
            // Prerequisites: These resources must exist
            string vnetName = "myVnet";
            string subnetName = "mySubnet";
            string diskEncryptionSetName = "myDES";
            
            // Construct resource IDs
            string subnetId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/virtualNetworks/{vnetName}/subnets/{subnetName}";
            string diskEncryptionSetId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/diskEncryptionSets/{diskEncryptionSetName}";
            
            string adminUsername = "azureuser";
            string sshPublicKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQC..."; // Replace with actual SSH public key

            var creator = new VmssCreator(credentials, subscriptionId);

            try
            {
                Console.WriteLine($"Creating VM Scale Set: {vmssName}");
                var vmss = await creator.CreateVmssWithEncryptedDataDiskAsync(
                    resourceGroupName,
                    location,
                    vmssName,
                    subnetId,
                    diskEncryptionSetId,
                    adminUsername,
                    sshPublicKey
                );

                Console.WriteLine($"Successfully created VMSS: {vmss.Name}");
                Console.WriteLine($"Provisioning State: {vmss.ProvisioningState}");
                Console.WriteLine($"Capacity: {vmss.Sku.Capacity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating VMSS: {ex.Message}");
                throw;
            }
        }
    }
}
