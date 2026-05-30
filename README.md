# API Inteligente de Tareas y Análisis

Esta es una API RESTful desarrollada en ASP.NET Core (.NET 8) para la gestión de tareas internas. Incluye integración con una API externa (JSONPlaceholder) y una funcionalidad de Inteligencia Artificial utilizando ML.NET para el análisis de sentimiento de comentarios.

## Pasos para ejecutar localmente

1. Clona este repositorio en tu máquina local.
2. Abre una terminal en la carpeta raíz del proyecto (`TareasApi`).
3. Restaura las dependencias del proyecto ejecutando:
   ```bash
   dotnet restore

4. Aplica las migraciones para generar la base de datos SQLite:  `dotnet ef database update`
5. Ejecuta la aplicación:  `dotnet run`
6. Abre tu navegador y navega a la URL indicada en la terminal agregando /swagger al final (por ejemplo: http://localhost:XXXX/swagger).

Comandos de migración:
Si necesitas generar la base de datos desde cero o agregar nuevas entidades, utiliza la herramienta CLI de Entity Framework Core:

Crear una nueva migración: `dotnet ef migrations add <NombreDeLaMigracion>`
Actualizar la base de datos: `dotnet ef database update`

Endpoints implementados:
Gestión de Tareas (CRUD y Filtros):
- GET /api/tareas - Lista todas las tareas. Permite filtrar por estado, prioridad, fechaInicio y fechaFin vía query parameters.
- GET /api/tareas/{id} - Obtiene una tarea específica por su ID.
- POST /api/tareas - Crea una nueva tarea (valida que la fecha de vencimiento no sea pasada).
- PUT /api/tareas/{id} - Actualiza una tarea existente.
- DELETE /api/tareas/{id} - Elimina una tarea.

Ejemplo de uso de la API externa
El endpoint GET /api/tareas-externas realiza una petición HTTP a https://jsonplaceholder.typicode.com/todos.
En lugar de devolver el JSON crudo original, la API captura la respuesta, maneja posibles errores de conexión (devolviendo un status 503 si el servicio externo falla) y mapea los datos a un Data Transfer Object (DTO) propio con la siguiente estructura:

`[
  {
    "externalId": 1,
    "titulo": "delectus aut autem",
    "completado": false
  }
]`

Explicación breve del modelo ML.NET usado
Para el análisis de sentimiento se implementó un modelo de Clasificación Binaria utilizando ML.NET:
- Algoritmo: Se utilizó SdcaLogisticRegression (Regresión Logística), ideal para clasificar texto en dos categorías.
- Transformación de datos: Se aplicó FeaturizeText para convertir las cadenas de texto (comentarios) en vectores numéricos (features) que el algoritmo puede procesar.
- Dataset: Se utilizó un dataset básico en memoria compuesto por frases positivas etiquetadas como true ("Positivo") y frases negativas etiquetadas como false ("Negativo").
- Flujo: Cuando un usuario envía un comentario, el modelo lo evalúa usando el motor de predicción (PredictionEngine) y devuelve el sentimiento detectado.
