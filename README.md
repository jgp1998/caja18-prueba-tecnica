# Prueba Técnica - Aplicación en ASP.NET Core - Caja 18
Este proyecto consiste en desarrollar una aplicación utilizando ASP.NET Core MVC. Se hace uso de arquitectura limpia y modular intentando respetar buenas practicas de codigo limpio para que el proyecto sea fácil de mantener y escalable.

## Aplicación desplegada

## Toma de decisiones
Se desarrollo en Visual Studio 2022 con ASP .NET core 8.0 MVC.
El proyecto lo dividí en capas con responsabilidades separadas.
Principalmente se uso Inyeccion de dependencias y capa de repositorio para interactuar con los datos de la api externa.
Para cumplir con los requerimientos me encargue solo de los endpoints de getall y getdetail.

## Levantar proyecto en local
1. **Clona el repositorio**
  git clone https://github.com/jgp1998/caja18-prueba-tecnica.git

2. **Abre el proyecto en Visual Studio**

- Abre Visual Studio.
   - Selecciona **Abrir un proyecto o solución** desde la pantalla de inicio.
   - Navega al directorio donde clonaste el repositorio y selecciona el archivo `*.sln` (archivo de solución) para abrir el proyecto.

3. **Configura las dependencias y configuraciones necesarias**
   - Visual Studio debería restaurar automáticamente las dependencias de NuGet. Si no es así, puedes hacerlo manualmente:
     - Haz clic derecho sobre la solución en el **Explorador de soluciones**.
     - Selecciona **Restaurar paquetes NuGet**.
   
   - **Configura el archivo `appsettings.json`** para la URL base de la API y otras configuraciones:
        ```json
     {
       "ApiSettings": {
         "BaseUrl": "https://api.restful-api.dev"
       }
     }
     ```

4. **Ejecuta el proyecto**
   - Una vez que hayas configurado las dependencias y los archivos de configuración, selecciona el proyecto de inicio en Visual Studio.
   - Haz clic en **Iniciar** o presiona `F5` para compilar y ejecutar la aplicación.

5. **Aplicación en el navegador**

     ```
     https://localhost:5001
     ```
