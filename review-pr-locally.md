## Review PR locally using copilot
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

### Helpers
```
git am --abort
```
