
Usage:
```
jkdl 1.0.0
Copyright (C) 2020 jkdl

  -b, --background    (Default: false) Start in background mode.

  -o, --overwride     (Default: false) Overwrite file in same name on download location.

  -m, --max           (Default: 3) Max number of concurrent download.

  -l, --location      (Default: .) Download location.

  -t, --trash         (Default: 1) Download thrash for change event in percentage.

  -p, --period        (Default: 1) Monitor refresh period in second.

  --help              Display this help screen.

  --version           Display version information.

```

Available commands in interactive (default) mode:
```
  link		insert link into the cache to download
  file		insert filename with links into the cache to download
  progress	print download progress
  stats		print download statistics
  history	print download history
  monitor	[start] or [stop] monitor download history
  cancel	cancel download by download identification
  exit		exit the application
  help		show this help message
```
