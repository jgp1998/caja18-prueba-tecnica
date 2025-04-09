#  DockerDemoApplication (.NET 8 + Docker)

Este proyecto es una aplicación de ejemplo desarrollada en .NET 8 que se ejecuta dentro de un contenedor Docker.

##  Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo
- Windows 11 (aunque también funciona en Linux/macOS)

##  Estructura del Proyecto

/DockerDemoApplication 
│ 
├── Dockerfile 
├── DockerDemoApplication.csproj 
└── Program.cs

##  Construcción de la imagen Docker

1. Abre una terminal en la carpeta raíz del proyecto (donde se encuentra el `Dockerfile`).
2. Ejecuta el siguiente comando para construir la imagen:


docker build -t dockerdemodotnet .

## Ejecución del contenedor
Para ejecutar la aplicación en un contenedor Docker:

docker run -d -p 8080:8081 --name aspcontainer dockerdemodotnet
