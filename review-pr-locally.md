## Create patch
```
### Create patch from the feature branch
git format-patch main..feature_branch
### Apply patch on the main branch
git apply --3way --whitespace=fix 0001-Describe-PrioritizedException.patch
### Click review button on the commit
```

## Make changes available for review vscode
```
checkout the branch locally
create a new branch based on that branch (just to be safe)
use git reset --soft $(git merge-base HEAD origin/main)
Then all the changes or staged and available for the #changes
```

## Use dotnet format to review the file
```
# install copilot cli
winget install GitHub.Copilot

# login
gh auth login

grep -Po 'CA\d+(?=.*= *error)' .editorconfig | paste -sd, -
dotnet format analyzers "./project.sln" --no-restore --verify-no-changes --include $(git diff --name-only HEAD -- '*.cs' | paste -sd' ' -) --report ./

# get rules with error severity from the file, use space seperation
grep -Po 'CA\d+(?=.*= *error)' .editorconfig | paste -sd' ' -

gh auth login
copilot -p "Read @format-report.json report, use data from "FileName", "FilePath", "FileChanges" fields to generate code review report in md format. create file with review report.
Use following template:
### Issue description
- **FilePath:**
- **Line(s):**
- **Problem:**
- *Review comment to add in the PR*" --yolo --model gpt-5.2 > output.md


# Get git changes, use space seperation
git diff --name-only HEAD -- '*.cs'
git diff --name-only HEAD -- '*.cs' | paste -sd' ' -
```

### Helpers
```
git am --abort
```
