<img width="800" height="603" alt="image" src="https://github.com/user-attachments/assets/4af46059-9c01-4ae4-ac59-a71ed81bea3b" />## Review PR locally using copilot
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
grep -Po 'CA\d+(?=.*= *error)' .editorconfig | paste -sd, -
dotnet format analyzers "./main/PromotionWorker.Main.sln" --no-restore --verify-no-changes --include "./main/Zerto.PromotionWorker.Accessors.JournalCache/JournalCacheAccessor.cs" --diagnostics $(grep -Po 'CA\d+(?=.*= *error)' .editorconfig | paste -sd' ' -)
```

### Helpers
```
git am --abort
```
