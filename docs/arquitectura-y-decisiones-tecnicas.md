# Arquitectura y decisiones técnicas

## 1. Propósito del documento

Este documento describe la arquitectura de InvoiceReminders, las principales decisiones de diseño, los motivos para adoptarlas y las limitaciones identificadas.

El objetivo es facilitar la revisión técnica del proyecto y explicar cómo se distribuyen las responsabilidades entre el frontend, el backend y la infraestructura.

El README contiene una visión general y las instrucciones básicas de ejecución. Este documento profundiza en las decisiones técnicas y sus consecuencias.

## 2. Arquitectura general

La aplicación se organiza en tres áreas principales:

### 2.1. Frontend

Construido con React y TypeScript.

Sus responsabilidades son:

- Presentar la información de las facturas.
- Consultar los clientes y sus facturas.
- Permitir iniciar el procesamiento de recordatorios.
- Mostrar los resultados y estados disponibles.
- Gestionar la comunicación HTTP con el backend.

El frontend no debe encargarse de enviar directamente los correos ni de actualizar la base de datos.

### 2.2. Backend

Construido con ASP.NET Core Web API.

Sus responsabilidades son:

- Exponer los endpoints HTTP.
- Consultar las facturas y los clientes.
- Identificar las facturas que requieren un recordatorio.
- Coordinar el envío de correos.
- Actualizar los estados en MongoDB.
- Gestionar errores y resultados del procesamiento.

### 2.3. Infraestructura

La solución utiliza:

- **MongoDB:** persistencia de las facturas y sus estados.
- **Mailpit:** servidor SMTP de desarrollo para capturar y visualizar los correos generados.

Mailpit permite comprobar los mensajes sin enviarlos a destinatarios reales.

## 3. Flujo de procesamiento de recordatorios

El procesamiento contempla dos transiciones:

| Estado actual | Acción | Nuevo estado esperado |
|---|---|---|
| `primerrecordatorio` | Enviar el correo del segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | Enviar el aviso de desactivación | `desactivado` |

El flujo general es:

1. La API obtiene las facturas que cumplen las condiciones de procesamiento.
2. Identifica la acción correspondiente al estado de cada factura.
3. Prepara y envía el correo mediante SMTP.
4. Si el envío es satisfactorio, intenta actualizar la factura en MongoDB.
5. La actualización comprueba el identificador y el estado esperado para reducir el riesgo de sobrescribir cambios concurrentes.
6. El procesamiento devuelve o registra el resultado según la implementación existente.

El envío del correo se realiza antes de cambiar el estado para evitar avanzar una factura cuando el envío falla.

Sin embargo, SMTP y MongoDB no comparten una transacción atómica. Por ejemplo, el correo podría enviarse correctamente y la actualización de MongoDB fallar. En ese escenario, el siguiente intento podría enviar el mismo correo de nuevo.

## 4. Decisiones técnicas del backend

### B1. ASP.NET Core Web API

**Decisión:** exponer las operaciones mediante una API HTTP.

**Motivo:** permite separar la interfaz de usuario de las reglas de procesamiento y facilita probar las operaciones del backend de manera independiente.

**Alternativa descartada:** implementar el procesamiento de correos y la modificación de estados directamente desde el frontend. Esto mezclaría responsabilidades y expondría detalles de infraestructura al navegador.

### B2. MongoDB como base de datos

**Decisión:** utilizar MongoDB.Driver para consultar y actualizar las facturas.

**Motivo:** permite trabajar con documentos y definir filtros específicos para recuperar y modificar los registros.

**Consecuencia:** la estructura de los documentos, los nombres de campos y los estados deben mantenerse consistentes con el código de la aplicación.

### B3. Actualización condicional de MongoDB

**Decisión:** realizar la actualización utilizando el identificador de la factura y el estado esperado.

**Motivo:** si otra ejecución cambia el estado antes de la actualización, el filtro puede impedir que una operación sobrescriba ese cambio.

**Beneficio:** añade una protección frente a ciertas condiciones de concurrencia.

**Limitación:** no garantiza por sí sola que nunca se envíe un correo duplicado. El envío y la persistencia son operaciones separadas.

### B4. Envío del correo antes de actualizar el estado

**Decisión:** enviar el correo y, si el envío termina correctamente, intentar actualizar el estado de la factura.

**Motivo:** evita marcar una factura como procesada cuando el envío falla antes de completarse.

**Consecuencia:** si el correo se envía y la actualización falla, el sistema puede quedar en un estado parcial.

**Mejora futura:** incorporar un mecanismo de idempotencia o un patrón Outbox para coordinar de forma más fiable los eventos pendientes de envío y su persistencia.

### B5. Control de concurrencia mediante `ClientRunGate`

**Decisión:** utilizar `ClientRunGate` para impedir ejecuciones simultáneas del procesamiento de un mismo cliente dentro de una instancia de la aplicación, si ese comportamiento corresponde a la implementación actual.

**Motivo:** reduce el solapamiento de procesos iniciados por solicitudes concurrentes.

**Limitación:** un control en memoria de una instancia no coordina varias réplicas de la API. En un despliegue distribuido se necesitaría un mecanismo compartido de coordinación o una estrategia de procesamiento con idempotencia.

### B6. Procesamiento por lotes y cursor

**Decisión documentada:** procesar las facturas por lotes y utilizar un cursor basado en `_id` para recorrer los resultados, si está implementado de esa forma.

**Motivo:** puede reducir el consumo de memoria y facilitar el procesamiento de un volumen mayor de facturas.

**Verificación necesaria:** confirmar en el código el tamaño real de lote, el filtro del cursor y el orden utilizado. La documentación final debe reflejar esos valores reales.

### B7. Índices de MongoDB

**Decisión documentada:** utilizar un índice compuesto por `clientId`, `status` y `_id`, si está definido en la configuración de la base de datos.

**Motivo:** un índice adecuado puede mejorar consultas que filtran por cliente y estado y recorren las facturas por identificador.

**Verificación necesaria:** confirmar que el índice se crea realmente en la implementación o en la configuración de MongoDB. No basta con mencionarlo en este documento.

### B8. Mailpit para el correo de desarrollo

**Decisión:** utilizar Mailpit como servidor SMTP local para las pruebas manuales.

**Motivo:** permite inspeccionar los correos generados sin entregarlos a destinatarios externos.

**Configuración documentada:**

- SMTP: `localhost:1025`
- Interfaz web: `http://localhost:8025`

El `docker-compose.yml` descrito para el proyecto define MongoDB, pero no Mailpit. Por lo tanto, Mailpit debe iniciarse por separado, salvo que la configuración real del repositorio se haya modificado.

### B9. Scheduler desactivado por defecto

**Decisión documentada:** mantener desactivado el procesamiento automático programado por defecto, si así está configurado en la aplicación.

**Motivo:** permite controlar cuándo se procesan los recordatorios durante el desarrollo y la evaluación técnica.

**Verificación necesaria:** confirmar el comportamiento predeterminado y la opción de configuración en el código real.

### B10. Pruebas automatizadas del backend

**Decisión:** utilizar xUnit y NSubstitute para probar la lógica del backend y sustituir dependencias cuando corresponda.

**Motivo:** permite comprobar reglas y escenarios de error sin depender necesariamente de una base de datos o servidor SMTP real en cada prueba.

**Limitación:** las pruebas unitarias no sustituyen las pruebas de integración de MongoDB, SMTP y API.

## 5. Decisiones técnicas del frontend

### F1. React y TypeScript estricto

**Decisión:** construir la interfaz con componentes React y TypeScript.

**Motivo:** facilita la reutilización de componentes y permite detectar inconsistencias de tipos durante el desarrollo.

El modo estricto de TypeScript debe mantenerse habilitado en la configuración del proyecto.

### F2. Organización por features

**Decisión:** agrupar el código relacionado por funcionalidad cuando esa sea la estructura utilizada en el repositorio.

**Motivo:** facilita encontrar los componentes, hooks, servicios y tipos relacionados con una misma funcionalidad.

Esta organización permite que los cambios de una funcionalidad se mantengan localizados, sin obligar a recorrer demasiadas carpetas globales.

Los elementos compartidos deben reservarse para funcionalidades que realmente se reutilicen en varias áreas.

### F3. TanStack React Query

**Decisión:** utilizar TanStack React Query para gestionar el estado de servidor en las consultas donde esté implementado.

**Motivo:** centraliza aspectos como los estados de carga, los errores, la caché y la actualización de consultas.

Esto reduce la necesidad de implementar manualmente el ciclo de vida de cada solicitud en distintos componentes.

### F4. Axios y la comunicación HTTP

**Decisión:** utilizar Axios para las solicitudes HTTP y mantener la comunicación con la API separada de la presentación.

**Motivo:** permite centralizar la configuración de las solicitudes y evitar que cada componente tenga que implementar los detalles de transporte.

### F5. Validación con Zod

**Decisión documentada:** utilizar Zod para validar los datos en los puntos donde la aplicación lo requiera y donde esté implementado.

**Motivo:** permite expresar reglas de validación mediante esquemas reutilizables.

**Verificación necesaria:** confirmar los formularios o respuestas que realmente utilizan Zod antes de afirmar que toda la aplicación valida sus datos con esta biblioteca.

### F6. Separación de responsabilidades de los componentes

**Decisión:** mantener los componentes de presentación y la lógica de datos separados cuando sea apropiado para la funcionalidad.

**Motivo:** facilita el mantenimiento, la reutilización y las pruebas.

Los componentes no deberían asumir responsabilidades innecesarias de comunicación HTTP o procesamiento de reglas de negocio.

### F7. Tailwind CSS y variantes de componentes

**Decisión:** utilizar Tailwind CSS para los estilos y CVA para gestionar variantes de componentes cuando corresponda a la implementación.

**Motivo:** ayuda a mantener consistencia visual y reduce la duplicación de estilos en componentes con diferentes variantes.

### F8. Pruebas del frontend

**Decisión:** utilizar Vitest y Testing Library para probar el comportamiento de la interfaz.

**Motivo:** permite comprobar resultados visibles e interacciones sin acoplar las pruebas innecesariamente a detalles internos de implementación.

## 6. SOLID y patrones de diseño

SOLID es un conjunto de principios para orientar el diseño del software. Su mención en este documento no implica que todas las clases cumplan perfectamente cada principio.

### Responsabilidad única

Cada componente o clase debería tener una responsabilidad clara y cambiar por razones relacionadas con ella.

En este proyecto, la separación entre la interfaz, la API y la infraestructura contribuye a mantener responsabilidades diferenciadas.

### Abierto/cerrado

El diseño debería permitir añadir comportamientos nuevos sin modificar innecesariamente partes estables del sistema.

La posibilidad de extender las acciones de recordatorio depende de cómo estén implementadas las reglas actuales.

### Sustitución de Liskov

Las implementaciones intercambiables deben respetar el contrato definido por sus abstracciones.

Este principio debe evaluarse a partir de las interfaces y clases que realmente existan en el proyecto.

### Segregación de interfaces

Los consumidores deberían depender solo de las operaciones que necesitan.

Este principio resulta relevante cuando existen interfaces con varias responsabilidades o múltiples consumidores.

### Inversión de dependencias

La lógica de aplicación debería depender de abstracciones cuando esa separación aporte valor, en lugar de acoplarse innecesariamente a detalles concretos de infraestructura.

La existencia y el alcance de esta separación deben verificarse en el código.

### Patrón Strategy

Strategy puede resultar útil si se requiere encapsular diferentes acciones de recordatorio en estrategias intercambiables.

Por ejemplo, cada estado podría asociarse con un comportamiento específico de envío y transición.

**Importante:** solo debe documentarse como patrón implementado si el código contiene realmente esas estrategias intercambiables. Si las reglas están resueltas mediante condiciones, Strategy debe presentarse como una posible mejora y no como una característica existente.

## 7. Riesgos y posibles mejoras

| Riesgo o limitación | Posible mejora |
|---|---|
| El correo y la actualización de MongoDB no son atómicos | Evaluar Outbox e idempotencia. |
| `ClientRunGate` puede limitarse a una instancia | Utilizar coordinación compartida si se despliega con varias réplicas. |
| Un fallo parcial puede requerir un nuevo intento | Mejorar el registro de resultados, los reintentos y la recuperación. |
| Las pruebas unitarias no cubren todas las integraciones | Añadir pruebas de integración con MongoDB y un servidor SMTP de prueba. |
| El procesamiento automático puede estar desactivado | Configurar explícitamente el scheduler y añadir observabilidad si se necesita ejecución programada. |

Estas mejoras son propuestas de evolución y no deben presentarse como implementadas si todavía no forman parte del código.

## 8. Verificaciones antes de la entrega

Antes de compartir este documento, revisar el código y confirmar:

- [ ] La estructura real del backend y del frontend.
- [ ] La implementación de `ClientRunGate`.
- [ ] El orden del envío del correo y la actualización del estado.
- [ ] El filtro utilizado en la actualización condicional.
- [ ] El tamaño de lote y el cursor, si existen.
- [ ] La definición real de los índices de MongoDB.
- [ ] El estado predeterminado del scheduler.
- [ ] El uso real de Zod y TanStack React Query.
- [ ] Si Strategy está implementado o solo es una mejora propuesta.
- [ ] Los resultados actuales de las pruebas.

La documentación debe describir lo que el código hace actualmente y diferenciarlo de las mejoras que podrían implementarse en el futuro.
