---
name: aws-knowledge-reference
description: Look up real-time AWS documentation, API references, best practices, and regional availability using the AWS Documentation MCP Server. Use this skill to verify AWS APIs, find working patterns, confirm regional support, and avoid hallucinated services, features, or SDK methods.
compatibility: Requires AWS Documentation MCP Server (https://awslabs.github.io/mcp/servers/aws-documentation-mcp-server)
---

# AWS Knowledge Reference

## Tools

| Need | Tool | Example |
|------|------|---------|
| Search AWS docs | `aws___search_documentation` | "EC2 RunInstances API parameters" |
| Read full doc page | `aws___read_documentation` | "https://docs.aws.amazon.com/AWSEC2/latest/APIReference/API_RunInstances.html" |
| Discover related docs | `aws___recommend` | "Amazon S3 encryption best practices" |
| List AWS regions | `aws___list_regions` | _no params_ |
| Check regional availability | `aws___get_regional_availability` | "EBS encryption ap-south-2" |

---

## Finding AWS Documentation

Use `aws___search_documentation` to find relevant AWS docs across:
- API references  
- Service guides  
- Best practices  
- Architecture patterns  

```
aws___search_documentation(query: "EC2 customer managed keys encryption")
aws___search_documentation(query: "S3 SSE-KMS bucket policy example")
aws___search_documentation(query: "IAM role for EC2 access to KMS")
```

Then fetch the full content when needed:

```
aws___read_documentation(url: "https://docs.aws.amazon.com/kms/latest/developerguide/overview.html")
```

**When to use:**
- Before writing AWS infrastructure or SDK code  
- When validating IAM permissions  
- When implementing security, encryption, networking, or scaling features  
- When unsure about API parameters or service behavior  

---

## AWS API Lookups

Use search to verify service APIs and parameters:

```
"RunInstances API EC2 parameters"
"CreateVolume EBS KMSKeyId"
"PutBucketEncryption S3"
"DescribeKey KMS API"
```

If multiple versions or pages appear, always fetch the authoritative reference using:

```
aws___read_documentation(url: "<docs.aws.amazon.com/...>")
```

---

## Regional Availability & Constraints

Always validate region support for:
- New AWS services  
- Features (e.g., Nitro Enclaves, Graviton, Local Zones)  
- Encryption capabilities  
- Instance families  

```
aws___list_regions()
aws___get_regional_availability(query: "EC2 Nitro Enclaves")
aws___get_regional_availability(query: "KMS multi-region keys")
aws___get_regional_availability(query: "EBS gp3")
```

---

## AWS Best Practices & Architecture Patterns

Use `aws___recommend` to expand research and find commonly associated guidance:

```
aws___recommend(topic: "EBS encryption with KMS")
aws___recommend(topic: "IAM least privilege EC2")
aws___recommend(topic: "Multi-account AWS architecture")
```

---

## Error & Design Troubleshooting

| Scenario | Query |
|--------|-------|
| Access denied | "KMS AccessDeniedException EC2 encryption" |
| API param mismatch | "RunInstances KmsKeyId parameter" |
| Feature unsupported | "Is gp3 supported in me-central-1" |
| IAM permission error | "kms:CreateGrant EC2 required permissions" |
| Service limits | "EBS volume limits per region" |

---

## Validation Workflow

1. Search docs  
2. Read full reference  
3. Check region support  
4. Explore best practices  

This workflow minimizes hallucinations and ensures production-grade correctness.
