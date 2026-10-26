# Despliegue local y guía de revisión técnica

## 1. Objetivo

Esta guía explica cómo preparar los servicios locales, ejecutar InvoiceReminders y comprobar el procesamiento de facturas y el envío de correos.

Los pasos están orientados al desarrollo y a la evaluación técnica local, no a un despliegue productivo en la nube.

## 2. Requisitos previos

Instala o verifica que estén disponibles:

- Git.
- .NET 10 SDK.
- Node.js y npm.
- Docker Desktop con Docker Compose.

Puedes comprobar las instalaciones con estos comandos:

```bash
git --version
dotnet --version
node --version
npm --version
docker --version
docker compose version
```

## 3. Obtener el repositorio

Si todavía no tienes el proyecto en tu equipo, clona el repositorio:

```bash
git clone <URL_DEL_REPOSITORIO>
cd InvoiceReminders
```

Reemplaza `<URL_DEL_REPOSITORIO>` por la URL real de GitHub.

Si ya tienes el proyecto descargado, abre una terminal en la carpeta raíz.

## 4. Iniciar MongoDB

Ubícate en la carpeta donde se encuentra `docker-compose.yml` y ejecuta:

```bash
docker compose up -d
```

Comprueba el estado de los servicios:

```bash
docker compose ps
```

Según la configuración documentada del proyecto, Docker Compose inicia únicamente MongoDB.

El puerto local documentado de MongoDB es `27017`, salvo que la configuración real del proyecto indique otro.

## 5. Iniciar Mailpit

Mailpit permite capturar los correos de prueba y visualizarlos desde el navegador.

**Importante:** Mailpit debe iniciarse por separado si no está definido en `docker-compose.yml`.

Ejemplo para iniciarlo mediante Docker:

```bash
docker run -d --name invoice_reminders_mailpit -p 1025:1025 -p 8025:8025 axllent/mailpit
```

Si ya existe un contenedor con ese nombre, no ejecutes nuevamente el comando de creación. Comprueba primero los contenedores existentes:

```bash
docker ps -a
```

Direcciones locales:

- SMTP: `localhost:1025`
- Interfaz web: `http://localhost:8025`

Verifica que el backend utilice el host y puerto SMTP correctos.

Si el backend se ejecuta dentro de otro contenedor, `localhost` hace referencia a ese contenedor. En ese caso, la configuración de red debe ajustarse.

## 6. Configurar y ejecutar el backend

Ubica el proyecto de ASP.NET Core que contiene el archivo `.csproj`.

Revisa la configuración de conexión a MongoDB y al servidor SMTP. Utiliza la configuración local prevista por el repositorio y evita incluir contraseñas, claves o datos sensibles en GitHub.

Desde la carpeta del proyecto de API, ejecuta:

```bash
dotnet restore
dotnet run
```

Si la solución contiene varios proyectos, especifica el proyecto de API:

```bash
dotnet run --project <RUTA_DEL_PROYECTO_API.csproj>
```

Durante la preparación se documentó el puerto `5080` para la API. Confirma la dirección exacta que aparece en la consola al iniciar el backend y utiliza esa dirección en el frontend y las pruebas.

## 7. Configurar y ejecutar el frontend

Abre otra terminal y entra en la carpeta del frontend.

Instala las dependencias:

```bash
npm install
```

Inicia el servidor de desarrollo:

```bash
npm run dev
```

La dirección documentada del frontend es:

`http://localhost:5173`

Comprueba que la configuración de Axios o del cliente HTTP utilice la URL correcta del backend.

### Otros comandos útiles

Si están definidos en `package.json`, puedes ejecutar:

```bash
npm run lint
npm run test
npm run build
```

Los nombres de los scripts dependen de la configuración real del proyecto. Si alguno no existe, consulta el apartado `scripts` del archivo `package.json`.

## 8. Verificar la aplicación manualmente

Con MongoDB, Mailpit, el backend y el frontend en ejecución:

1. Abre `http://localhost:5173`.
2. Comprueba que se visualicen las facturas.
3. Comprueba que se carguen los clientes.
4. Selecciona un cliente si la interfaz permite filtrar por cliente.
5. Ejecuta el procesamiento de recordatorios.
6. Abre `http://localhost:8025`.
7. Verifica que Mailpit haya capturado los correos correspondientes.
8. Consulta nuevamente las facturas y comprueba los estados.
9. Revisa la consola del backend y la consola del navegador si aparece algún error.

### Transiciones esperadas

| Estado inicial | Acción esperada | Estado posterior esperado |
|---|---|---|
| `primerrecordatorio` | Envío del segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | Envío del aviso de desactivación | `desactivado` |

Para realizar esta prueba, utiliza facturas de prueba con los campos y estados que espera la aplicación.

No utilices datos reales de clientes ni direcciones de correo reales para probar el flujo.

## 9. Endpoints principales

Utiliza la URL base que muestre el backend al iniciar. Si el puerto es el documentado, la base sería `http://localhost:5080`.

| Método | Endpoint | Descripción |
|---|---|---|
| `GET` | `/api/invoices` | Consultar facturas. |
| `GET` | `/api/invoices?clientId=acme` | Consultar facturas por cliente. |
| `GET` | `/api/clients` | Consultar clientes. |
| `POST` | `/api/reminders/process` | Procesar recordatorios de todos los clientes. |
| `POST` | `/api/clients/{clientId}/reminders/process` | Procesar recordatorios de un cliente. |

Si Swagger está habilitado en el backend, también puedes utilizarlo para explorar los endpoints y realizar solicitudes de prueba.

## 10. Ejecutar las pruebas automatizadas

### Backend

Desde la raíz de la solución o desde la carpeta correspondiente al proyecto de pruebas:

```bash
dotnet test
```

### Frontend

Desde la carpeta del frontend:

```bash
npm test
```

Si `npm test` no está definido, revisa `package.json` para conocer el comando configurado para Vitest.

Durante la preparación de esta documentación se reportaron 36 pruebas de backend y 25 de frontend aprobadas. Ejecuta nuevamente las pruebas antes de la entrega y registra los resultados reales del commit que vas a compartir.

Las pruebas unitarias no sustituyen una prueba de integración completa con MongoDB y SMTP.

## 11. Solución de problemas

### La API no conecta con MongoDB

- Comprueba que Docker Desktop esté iniciado.
- Ejecuta `docker compose ps`.
- Revisa el puerto y la cadena de conexión de MongoDB.
- Confirma que el backend esté utilizando la configuración correcta.

### No aparecen correos en Mailpit

- Comprueba que el contenedor de Mailpit esté activo.
- Abre `http://localhost:8025`.
- Verifica que el backend esté apuntando al puerto SMTP `1025`.
- Comprueba que la configuración de red sea correcta para el entorno donde corre el backend.

### El frontend no muestra información

- Comprueba que el backend esté en ejecución.
- Verifica la URL base configurada en el cliente HTTP.
- Revisa la consola del navegador y la pestaña de red.
- Revisa los errores de la consola del backend.

### El estado de una factura no cambia

- Comprueba el estado inicial de la factura.
- Verifica que el correo se haya enviado correctamente.
- Revisa los errores de MongoDB.
- Comprueba el resultado de la actualización condicional.
- Confirma que la factura cumple los criterios de procesamiento.

### Un puerto está ocupado

Comprueba los procesos y contenedores que utilizan ese puerto antes de cambiar la configuración. Si cambias un puerto, actualiza también las configuraciones que dependen de él.

## 12. Lista de verificación para la entrega

- [ ] El proyecto se clona desde el repositorio compartido.
- [ ] MongoDB se inicia correctamente.
- [ ] Mailpit se inicia correctamente.
- [ ] El backend arranca sin errores.
- [ ] El frontend arranca sin errores.
- [ ] Las facturas y los clientes se visualizan.
- [ ] El procesamiento genera los correos esperados.
- [ ] Los estados se actualizan como corresponde.
- [ ] Las pruebas automatizadas se ejecutan correctamente.
- [ ] Los comandos y las rutas se verificaron contra el repositorio real.
- [ ] No se publicaron secretos ni datos reales.
- [ ] El README enlaza correctamente los documentos de esta carpeta.
