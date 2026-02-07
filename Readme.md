# Enable vscode Skill
In the settinfs find skills and enable them

# context 7
## Install
1. Get API KEY
2. add to mcp https://context7.com/docs/resources/all-clients#remote-server-connection-6
```
"context7": {
			"type": "http",
			"url": "https://mcp.context7.com/mcp",
			"headers": {
				"CONTEXT7_API_KEY": "${input:CONTEXT7_API_KEY}"
			}
		}
```

## usage

Implement basic authentication with Supabase. use library `/dotnet/aspnetcore/v8.0.21` for API and docs.