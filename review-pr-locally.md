## Review PR locally using copilot
### Create patch from the feature branch
```
git format-patch main..feature_branch
```
### Apply patch on the main branch
```
git apply --3way --whitespace=fix 0001-Describe-PrioritizedException.patch
```

### Click review button on the commit

### Helpers
```
git am --abort
```
