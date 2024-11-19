SET GOOS=linux
SET GOARCH=amd64
SET CGO_ENABLED=0
go build -o file  main.go