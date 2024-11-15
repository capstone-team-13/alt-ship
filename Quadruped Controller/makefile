.PHONY: build clean server

build:
	cmake -B build -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Debug -DCMAKE_EXPORT_COMPILE_COMMANDS=ON
	cmake --build build --target Server

clean:
	if exist build rmdir /s /q build

server: build
	.\build\server\Server.exe

