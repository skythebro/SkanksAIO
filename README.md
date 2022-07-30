# 

Honourable mentions for contributions before this repo was made with the current version: https://github.com/AviiNL/ & https://github.com/NopeyBoi

v1 

# Using the Git Hook

Copy/Paste `.env.example` to `.env` and change the variable to your unhollowed directory

Copy the `hooks/post-merge` file to `.git/hooks/post-merge`

> **NOTE:** `.git` is a hidden directory, make sure to have Show Hidden Files enabled.

Or use git bash:
```
cp hooks/post-merge .git/hooks/post-merge
```

The first time after performing the above steps, you need to manually run the hook:

In Git bash, from the project root folder run:
```
.git/hooks/post-merge
```

## Building

If all is well, the project should build without errors, and `FodyWeavers` should have packed all dependencies.

Start the server.

Edit configuration.

Restart the server.
