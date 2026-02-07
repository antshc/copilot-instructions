# AWS .NET SDK – Effective Prompt Usage Examples

These examples demonstrate how to use the `aws-implement` prompt effectively to generate
production-ready C# code using pinned AWS SDK versions from `Directory.Packages.props`
and verified AWS documentation via MCP.

---

## Example 1: Create Encrypted EBS Volume with CMK and Attach to EC2

**When to use:**  
Implementing core storage encryption with AWS KMS and EBS using AWS SDK for .NET.

**Prompt:**
```text
Task:
Write C# code that creates a new EBS volume encrypted with a specific customer-managed KMS key
and attaches it to an existing EC2 instance.

Details:
- The volume type should be gp3
- The KMS key ARN is provided as a parameter
- The EC2 instance ID is provided as a parameter
- The code must work with the AWS SDK package versions defined in Directory.Packages.props
```

**Why this is effective:**
- Exercises EC2 + EBS + KMS integration
- Forces SDK version awareness (no “latest SDK” drift)
- Validates encryption parameters against official docs

---

## Example 2: Launch Template with Encrypted Root Volume for Auto Scaling

**When to use:**  
Provisioning secure EC2 instances via Launch Templates and Auto Scaling Groups.

**Prompt:**
```text
Task:
Generate .NET code that creates or updates an EC2 Launch Template where:
- The root EBS volume is encrypted using a customer-managed KMS key
- The volume type is gp3
- The Launch Template is used by an Auto Scaling Group

Constraints:
- Must be compatible with the AWS SDK versions in Directory.Packages.props
- Do not use APIs or properties introduced in newer SDK versions
- Validate parameters (Encrypted, KmsKeyId, BlockDeviceMappings) using aws___read_documentation
```

**Why this is effective:**
- Covers nested request models (LaunchTemplateData, BlockDeviceMappings)
- Catches hallucinated properties like `KmsKeyArn` vs `KmsKeyId`
- Matches real-world production provisioning patterns

---

## Example 3: Create KMS Grant for EC2 to Use CMK (Fix AccessDenied)

**When to use:**  
Debugging permission issues when EC2 cannot use a CMK for EBS encryption.

**Prompt:**
```text
Task:
Write C# code that:
- Creates a KMS grant allowing EC2 to use a customer-managed key for EBS encryption
- Verifies the KMS key exists before creating the grant
- Uses IAM role-based authentication (no static credentials)

Rules:
- If CreateGrant parameters differ in the pinned SDK version, call it out explicitly

```

**Why this is effective:**
- Targets a common real-world failure mode (KMS AccessDenied)
- Forces correct usage of CreateGrantRequest fields for the pinned SDK version
- Separates IAM policy concerns from KMS key policy/grants

---

## Reusable Pattern

```text
Task:
<Concrete AWS functionality>

Constraints:
- Must respect Directory.Packages.props versions
- Must validate AWS API behavior via aws___search_documentation / aws___read_documentation

Output:
- Fully compilable C# code
- No pseudocode
- No new NuGet packages
```
