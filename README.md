# InvoiceReminders

Aplicación full stack para consultar facturas y procesar recordatorios de pago por cliente. El sistema identifica las facturas que requieren un recordatorio, envía el correo correspondiente y actualiza su estado en MongoDB cuando el envío se realiza correctamente.

## Tecnologías

**Backend**
- .NET 10
- C#
- ASP.NET Core Web API
- MongoDB.Driver
- MailKit
- xUnit y NSubstitute

**Frontend**
- React 19
- TypeScript estricto
- Vite
- Tailwind CSS v4
- TanStack React Query
- Axios
- Zod
- CVA
- React Icons
- Vitest y Testing Library
- ESLint y Prettier

**Infraestructura**
- Docker y Docker Compose
- MongoDB
- Mailpit para capturar correos en desarrollo

## Arquitectura general

El proyecto separa la interfaz de usuario, la API y los servicios de infraestructura.

```text
React + TypeScript
        |
        | HTTP
        v
ASP.NET Core Web API
        |
        +------ MongoDB
        |
        +------ Mailpit (SMTP local)
```

- **Frontend:** presenta las facturas, permite consultar por cliente y ejecutar el procesamiento de recordatorios.
- **Backend:** expone los endpoints y coordina el procesamiento, el envío de correos y la actualización de estados.
- **MongoDB:** almacena las facturas y sus estados.
- **Mailpit:** captura los correos de prueba para poder inspeccionarlos sin enviarlos a destinatarios externos.

Para conocer la organización interna y la justificación de las decisiones de diseño, consulta [Arquitectura y decisiones técnicas](docs/arquitectura-y-decisiones-tecnicas.md).

Para instalar, ejecutar y verificar la aplicación, consulta la [Guía de despliegue local](docs/despliegue.md).

## Funcionalidades principales

- Consultar las facturas registradas.
- Filtrar las facturas por cliente.
- Consultar los clientes disponibles.
- Procesar recordatorios para todos los clientes.
- Procesar recordatorios para un cliente específico.
- Enviar correos mediante SMTP.
- Consultar los correos generados en Mailpit.
- Visualizar los estados de las facturas desde la interfaz.

## Flujo de recordatorios

El sistema contempla las siguientes transiciones:

| Estado inicial | Acción | Estado esperado |
|---|---|---|
| `primerrecordatorio` | Enviar correo de segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | Enviar aviso de desactivación | `desactivado` |

El estado se actualiza después de un envío satisfactorio. La actualización condicional de MongoDB ayuda a proteger frente a cambios concurrentes, aunque el envío de correo y la actualización de la base de datos no constituyen una única transacción.

Los límites y las posibles mejoras de este flujo se explican en el documento de arquitectura.

## Endpoints principales

| Método | Endpoint | Descripción |
|---|---|---|
| `GET` | `/api/invoices` | Consulta las facturas. |
| `GET` | `/api/invoices?clientId=acme` | Consulta las facturas de un cliente. |
| `GET` | `/api/clients` | Consulta los clientes disponibles. |
| `POST` | `/api/reminders/process` | Procesa los recordatorios de todos los clientes. |
| `POST` | `/api/clients/{clientId}/reminders/process` | Procesa los recordatorios de un cliente. |

La URL base depende de la configuración del backend. Durante la preparación del proyecto se utilizó el puerto `5080`.

## Estructura del repositorio

```text
InvoiceReminders/
├── backend/   # API y pruebas del backend
├── frontend/  # Aplicación React
├── docs/
│   ├── arquitectura-y-decisiones-tecnicas.md
│   └── despliegue.md
├── docker-compose.yml
└── README.md
```

**Nota:** los nombres de las carpetas anteriores son una referencia. Conserva en este apartado los nombres reales de las carpetas de tu repositorio.

## Requisitos previos

- Git.
- .NET 10 SDK.
- Node.js y npm.
- Docker Desktop con Docker Compose.

## Ejecución rápida

1. Inicia MongoDB desde la carpeta que contiene `docker-compose.yml`:

   ```bash
   docker compose up -d
   ```

2. Inicia Mailpit por separado, si todavía no está ejecutándose.
3. Configura la conexión del backend a MongoDB y al servidor SMTP de Mailpit.
4. Ejecuta el backend con `dotnet run` desde el proyecto de la API.
5. Ejecuta `npm install` y `npm run dev` desde la carpeta del frontend.
6. Abre el frontend en `http://localhost:5173`.
7. Abre Mailpit en `http://localhost:8025` para revisar los correos.

**Importante:** el `docker-compose.yml` descrito para este proyecto inicia únicamente MongoDB. Mailpit debe iniciarse por separado. Los comandos completos se encuentran en [`docs/despliegue.md`](docs/despliegue.md).

## Pruebas

Backend:

```bash
dotnet test
```

Frontend:

```bash
npm test
```

Utiliza los comandos desde las carpetas o proyectos correspondientes y confirma que los scripts estén definidos en el repositorio.

Durante la preparación de la documentación se reportaron 36 pruebas de backend y 25 pruebas de frontend aprobadas. Ejecuta nuevamente las pruebas antes de entregar el proyecto para confirmar los resultados del commit final.

## Limitaciones conocidas

El envío de correo y la actualización de MongoDB son operaciones independientes. Si el correo se envía correctamente, pero la actualización falla, una ejecución posterior podría volver a enviar el mismo correo.

Para un entorno productivo sería conveniente evaluar mecanismos adicionales de idempotencia, reintentos controlados y un patrón Outbox.

Consulta [`docs/arquitectura-y-decisiones-tecnicas.md`](docs/arquitectura-y-decisiones-tecnicas.md) para ver el análisis de estas limitaciones y las posibles mejoras.
