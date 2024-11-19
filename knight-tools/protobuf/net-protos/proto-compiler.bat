chcp  65001

REM 编译.proto文件，转换为C#，输出到当前目录下
@echo compile proto to C#

@call protoc.exe --csharp_out ./../../../knight-client/Assets/Game.Config/Protos/Generate ./protos/GameNetProto.proto