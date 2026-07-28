# Petshop Sindy Web — Documentación del proyecto

> Documento vivo. Se actualiza a medida que se toman decisiones. Última actualización: 23/07/2026.

## 1. Contexto del negocio

Sindy es un petshop de barrio (barrio/ciudad, no cadena), con clientela ya establecida. El proyecto es para la hermana del desarrollador. Hay una posible expansión a una segunda sucursal en el futuro, pero no es prioridad actual — el sistema se diseña para una sola sucursal por ahora.

**Objetivo del proyecto**: cubrir una necesidad real del negocio (sistema de ventas online) y a la vez servir como proyecto de aprendizaje para el desarrollador (estudiante avanzado de Análisis de Sistemas), documentando todo el proceso.

**Plazos**: sin fecha límite de lanzamiento definida.

## 2. Alcance funcional (MVP)

- Catálogo de productos con categorías (alimentos, accesorios, medicamentos, etc.)
- Productos con **variantes** (comida por peso, juguetes por color, huesos de cuero por medida, collares por medida)
- Cuenta de usuario **obligatoria** para comprar (no hay checkout como invitado)
- Carrito de compras
- Checkout con dos métodos de entrega: **retiro en el local** y **envío a domicilio** (zona local únicamente, sin envíos a otras ciudades)
- Pago único (sin cuotas/financiación)
- Pasarela de pagos: MercadoPago
- Comprobante interno no fiscal (Sindy es monotributista; la integración con AFIP/ARCA para factura electrónica queda como **módulo futuro separado**, no bloquea el MVP)
- Panel de administración para que los dueños (no técnicos) carguen y gestionen productos/pedidos día a día

**Fuera del MVP (a futuro)**:
- Bot de Telegram (proyecto aparte, encarado por el desarrollador en paralelo — uso interno, responde preguntas y deriva)
- Sistema de fidelización / puntos
- Integración de facturación electrónica AFIP/ARCA
- Soporte multi-sucursal

## 3. Stack tecnológico

| Capa | Tecnología |
|---|---|
| Frontend | React + Vite + Tailwind CSS |
| Backend | ASP.NET Core / .NET 10 |
| Acceso a datos | Entity Framework Core |
| Base de datos (desarrollo) | SQLite |
| Base de datos (producción, futuro) | PostgreSQL administrado |
| Contenedores | Docker |
| Despliegue (pruebas) | Servidor propio con Coolify |
| Despliegue (a futuro) | A definir — posible frontend en Cloudflare Pages/Vercel, backend en PaaS (Railway/Render/Fly.io/Azure), DB administrada |
| Herramienta de codificación | Google Antigravity (IDE agéntico) — se usará una vez cerradas las decisiones de arquitectura acá |

**Decisión clave de portabilidad**: se usa el patrón **Repository Pattern** para que el acceso a datos esté desacoplado. Esto permite migrar de SQLite a PostgreSQL cambiando solo la implementación del repositorio y el connection string, sin tocar lógica de negocio.

## 4. Arquitectura

### 4.1 Organización del repositorio

Se decidió **monorepo** (frontend y backend en el mismo repositorio Git), por simplicidad dado que es un desarrollador único y facilita versionado sincronizado y despliegue con un solo `docker-compose.yml`.

```
petshop-sindy/
├── backend/
│   ├── SindyPetshop.Api/              # Controllers, Program.cs, DTOs
│   ├── SindyPetshop.Domain/           # Entidades puras (Producto, Pedido, etc.)
│   ├── SindyPetshop.Infrastructure/   # EF Core, DbContext, Repositories
│   ├── SindyPetshop.Application/      # Services, lógica de negocio
│   └── SindyPetshop.Api.sln
├── frontend/
│   ├── src/
│   │   ├── components/                # Componentes reutilizables
│   │   ├── pages/                     # Catalogo, Carrito, Login, Admin
│   │   ├── services/                  # Llamadas a la API
│   │   ├── hooks/
│   │   └── context/                   # Estado global (carrito, auth)
│   └── package.json
├── docker-compose.yml
├── docs/
└── README.md
```

### 4.2 Capas del backend (Clean Architecture simplificada)

Flujo de una request: `Frontend → Controller → Service → Repository → EF Core/SQLite`

Cada capa solo conoce a la inmediata inferior — el Controller nunca accede directo a la base de datos, y el Service no conoce detalles de HTTP. El `Domain` (entidades) es compartido por todas las capas y no depende de EF Core ni de ASP.NET.

## 4.3 Repositorio

Código en GitHub: `https://github.com/KvKeev/petshop-sindy` (rama `main`). Regla de trabajo: no se ejecutan ni generan cambios de código sin confirmación directa del desarrollador.

## 4.4 Contrato de la API — Módulo Catálogo

- Rutas públicas: listado paginado y filtrado (`GET /api/v1/productos`), detalle (`GET /api/v1/productos/{id}`), listado de categorías
- Rutas administrativas (protegidas por rol): creación, actualización y borrado lógico de productos/variantes

## 4.5 Progreso técnico (backend)

- [x] Solución .NET con 4 proyectos en capas (Domain, Application, Infrastructure, Api) y referencias correctas entre ellos
- [x] Entidades del Domain: `Categoria`, `Producto`, `VarianteProducto`, `HistorialStock`
- [x] `DbContext` con SQLite, primera migración (`InitialCreate`) aplicada — `sindypetshop.db` generado
- [x] Documentación de API con Scalar (`/scalar/v1`), no Swashbuckle (conflicto con soporte nativo de OpenApi de .NET 10)
- [x] Repository Pattern: `IRepository<T>` genérico + `IProductoRepository` en Domain, `RepositoryBase<T>` + `ProductoRepository` en Infrastructure
- [x] DTOs desacoplados de las entidades (`ProductoDto`, `ProductoDetalleDto`, `VarianteProductoDto`, `CategoriaDto`, `PagedResult<T>`) — nunca se expone `StockFisico` por la API pública, solo `StockDisponibleWeb`
- [x] `ProductoService` (Application) traduce entidades a DTOs
- [x] `ProductosController` con `GET /api/v1/productos` (paginado, filtro por categoría) y `GET /api/v1/productos/{id}` (detalle con variantes)
- [x] `CategoriasController` con `GET /api/v1/categorias`
- [x] `DataSeeder` con datos de prueba (2 categorías, 2 productos, variantes incluyendo un caso de fraccionamiento configurado)
- [x] **Módulo de catálogo (lectura) verificado end-to-end con datos reales**: listado, filtro por categoría, detalle con variantes, y cálculo correcto de `StockDisponibleWeb`
- [x] Entidades `Cliente`, `Direccion`, `Pedido`, `DetallePedido` creadas y migradas
- [x] Autenticación: `Cliente.PasswordHash` con BCrypt, JWT propio (sin ASP.NET Core Identity, decisión consciente por simplicidad y valor de aprendizaje), `AuthController` con `/registro` y `/login`, secreto JWT gestionado con User Secrets (nunca en el repo) — **verificado: registro, duplicado de email (409), login correcto, login con password incorrecta (401)**
- [x] Entidad `Mascota` (Nombre, Tipo enum, relación con Cliente), endpoints `GET/POST /api/v1/mascotas` protegidos con `[Authorize]` (ClienteId se extrae del JWT, nunca del body)
- [x] Trazabilidad de compras por mascota: `DetallePedido.MascotaId` (opcional) + `Pedido.Origen` (Web/Mostrador). Endpoint `GET /api/v1/mascotas/{id}/historial` — **verificado end-to-end con JWT en Scalar**
- [x] Seguridad: `GetHistorial` ahora valida que solo el dueño de la mascota o un usuario con rol Admin puedan consultar su historial (403 Forbidden en caso contrario, 404 si no existe) — **verificado con dos usuarios distintos**

## 5. Modelo de datos (entidades confirmadas)

- **Categoria**: agrupa productos (alimentos, accesorios, medicamentos, etc.)
- **Producto**: nombre, descripción, categoría, imagen (`imagen_url`), activo/inactivo
- **VarianteProducto**: cada producto tiene una o más variantes, con atributo genérico (`atributo`: "Peso"/"Color"/"Medida", `valor`: "1kg"/"Rojo"/"M"), precio propio. Todo producto necesita al menos una variante, aunque sea "Standard". Maneja stock y fraccionamiento (ver 5.1).
- **HistorialStock**: registro de auditoría de cada movimiento de stock (venta, fraccionamiento, ajuste manual, carga inicial) por variante — necesario para trazabilidad, no reconstruible retroactivamente si no se implementa desde el inicio.
- **Cliente**: nombre, email, password (cuenta obligatoria)
- **Direccion**: asociada a un cliente, puede tener varias
- **Pedido**: cliente, dirección (si aplica), fecha, estado, método de entrega (retiro/envío), **origen** (Web/Mostrador — soporta ventas de mostrador a futuro sin tabla paralela), total
- **DetallePedido**: pedido, variante de producto, **mascota asociada (opcional)** — permite reconstruir "qué compró esta mascota" sin campo fijo, cantidad, precio unitario al momento de la compra (no se recalcula si el precio cambia después)
- **Mascota**: nombre, tipo (enum: Perro/Gato/Ave/Otro), asociada a un cliente (1 a N) — se carga desde el perfil, no en el registro

### 5.1 Gestión de stock y fraccionamiento (venta de alimento a granel)

Problema real a resolver: evitar vender online una bolsa que se acaba de abrir en el local, y permitir fraccionar bolsas cerradas en presentaciones sueltas (ej. 1kg, 3kg) sin romper la trazabilidad.

**Stock con buffer (reserva para mostrador):**
- `stock_fisico` (int): cantidad real en el local
- `stock_minimo_web` (int): buffer no expuesto a la web (default configurable)
- `StockDisponibleWeb` (calculado en C#, no es columna de DB): `max(0, stock_fisico - stock_minimo_web)`

**Fraccionamiento:**
- `es_fraccionable` (bool): si esta variante (ej. Bolsa 20kg) se puede abrir para vender suelta
- `variante_destino_id` (int, FK nullable, auto-referencia a `VarianteProducto`): a qué variante suelta se transforma. Relación uno-a-muchos (muchas variantes origen pueden apuntar al mismo destino, cada origen apunta a un solo destino)
- `cantidad_fraccionable` (int, nullable): cantidad que se suma a la variante destino al fraccionar (ej: 20)
- Venta en la web solo en presentaciones fijas predefinidas (no a granel libre), para mantener la logística simple

**Reglas de implementación:**
- La operación de fraccionar (restar de origen + sumar a destino + registrar en `HistorialStock`) debe ejecutarse como una única transacción atómica en la base de datos, para evitar estados inconsistentes si falla a mitad de camino
- Todo movimiento de stock (venta, fraccionamiento, ajuste) debe quedar registrado en `HistorialStock`

## 6. Decisiones ya tomadas

- Backend en .NET 10 (no Node.js), por formación académica y tooling de EF Core
- Facturación: comprobante interno en el MVP, AFIP como módulo futuro
- Variantes de producto: sí, necesarias desde el MVP
- Cuenta obligatoria para comprar (no hay checkout invitado)
- Envíos solo en zona local (no envíos a otras ciudades)
- Ambas opciones de entrega: retiro en local y envío a domicilio
- Pago único, sin cuotas
- Monorepo
- Repository Pattern para portabilidad de base de datos
- Herramienta de codificación: Google Antigravity (se descartó Claude Code por ser pago)
- Repositorio en GitHub creado y vinculado: `https://github.com/KvKeev/petshop-sindy`
- Estrategia de desarrollo: backend-first, de adentro hacia afuera (Domain → Infrastructure → Application → API)
- Stock con buffer web/local + fraccionamiento de bolsas + historial de movimientos para trazabilidad (ver sección 5.1)
- Encuesta de relevamiento armada y enviada a la dueña

## 7. Pendiente de relevar con el negocio (Sindy)

### Checkout y pagos
- [ ] ¿El pago es siempre online (incluso retirando en el local) o se permite pagar al retirar?
- [ ] Costo de envío: ¿tarifa fija o varía por zona/distancia?
- [ ] ¿Aceptan transferencia bancaria o efectivo además de MercadoPago?
- [ ] ¿La cuenta de MercadoPago ya existe o hay que crearla?

### Catálogo
- [ ] Cantidad aproximada de productos actuales
- [ ] ¿Manejan código de barras o SKU?
- [ ] ¿Venden productos que requieren receta/autorización (medicamentos veterinarios)?
- [ ] ¿Cómo actualizan el stock hoy? (a mano, Excel, otro sistema)
- [ ] Estado del Excel actual: ¿actualizado o con datos viejos/duplicados?
- [ ] ¿Manejan proveedores en alguna planilla?

### Clientes
- [ ] ¿Tienen base de clientes existente (WhatsApp, contactos) para migrar?
- [ ] ¿Interés en sistema de fidelización a futuro?

### Facturación
- [ ] ¿Emiten factura por cada venta actualmente, o solo a veces?
- [ ] ¿Usan algún sistema de facturación ya (app AFIP, Alegra, Contabilium)?

### Marca e identidad
- [x] Logo y paleta de colores: **confirmado, ya disponibles**
- [ ] ¿Tienen dominio propio comprado?

### Nuevos módulos a evaluar
- [ ] Servicio de peluquería: pendiente de definir alcance con la dueña antes de contemplarlo como módulo (¿reservas de turnos? ¿solo informativo?)
- [x] **Mascotas del cliente**: entidad `Mascota` (Nombre, Tipo enum: Perro/Gato/Ave/Otro, relación 1 a N con Cliente). Se agrega **después** del registro, desde el perfil.
- [ ] **Trazabilidad de "qué come cada mascota"**: necesidad real de negocio — en un petshop barrial, no todas las empleadas conocen a cada cliente/mascota como la dueña. Solución de diseño: `DetallePedido` tiene `MascotaId` opcional (para qué mascota fue esa línea de compra); no se guarda un campo fijo de "alimento habitual" (una mascota puede tener varios: alimento + snacks + medicamento), se consulta el historial de compras de esa mascota bajo demanda.
- [ ] **Venta de mostrador**: por ahora el mostrador sigue siendo manual/informal (no entra al sistema). Riesgo: stock del sistema se desincroniza con la realidad, e historial de "qué come cada mascota" queda incompleto para clientes que compran solo en persona. **Plan recomendado**: antes de una integración total, construir una pantalla simple en el panel admin ("Registrar venta de mostrador") que cree un `Pedido` con `Origen = Mostrador`, descuente stock real y registre `HistorialStock` — puente liviano sin necesitar carrito/pago/envío. Prioridad: después del carrito/checkout web.

## 8. Seguridad — checklist a implementar por módulo

Se aplica progresivamente a medida que se construye cada módulo, no todo de una vez.

### Acceso a datos
- [ ] Usar siempre EF Core con LINQ (parametrización automática) — nunca concatenar input del usuario en SQL crudo
- [ ] Si se necesita SQL crudo alguna vez, usar `FromSql` con interpolación tipada, nunca `FromSqlRaw` con strings concatenados

### Autenticación (módulo login/registro)
- [ ] Hash de contraseñas con bcrypt o Argon2 (ASP.NET Core Identity lo resuelve out-of-the-box)
- [ ] Rate limiting en el endpoint de login para prevenir fuerza bruta
- [ ] JWT para autenticación de API (evita depender de cookies, reduce superficie de CSRF)

### Autorización (todos los endpoints, especialmente panel admin)
- [ ] Validar rol (Cliente vs Admin) en el backend en cada endpoint sensible — nunca confiar en que el frontend "esconde" opciones
- [ ] Nunca asumir que un usuario logueado tiene permiso para todo

### Validación de entrada
- [ ] Validar tipo, longitud y formato de todo dato recibido en la API (no confiar solo en validación del frontend)

### Datos sensibles
- [ ] Nunca loguear ni devolver contraseñas, hashes, o tokens completos en respuestas de la API
- [ ] No manejar datos de tarjetas directamente — eso lo resuelve MercadoPago (evita alcance de cumplimiento PCI-DSS)

### Gestión de secretos
- [ ] Connection strings, JWT secret, API keys de MercadoPago: en variables de entorno, nunca hardcodeados ni commiteados a Git
- [ ] `.gitignore` configurado desde el primer commit para excluir `appsettings.Development.json` y archivos `.env`

### Infraestructura
- [ ] HTTPS obligatorio en producción
- [ ] CORS configurado para aceptar solo el dominio real del frontend
- [ ] XSS: cuidado con `dangerouslySetInnerHTML` en React; por defecto React ya escapa contenido

## 10. Notas de entorno (multi-máquina)

Al clonar el repo en una máquina nueva, hay 3 cosas que **no viajan con Git** (a propósito) y hay que recrear:
1. `dotnet ef database update` — recrea el archivo `.db` local aplicando las migraciones versionadas
2. `dotnet user-secrets set "Jwt:Secret" "..."` (parado en `SindyPetshop.Api`) — el secreto de JWT es específico de cada entorno
3. `dotnet dev-certs https --trust` — certificado HTTPS de desarrollo (en Linux puede no confiar automáticamente en el navegador; si pasa, exportar `SSL_CERT_DIR="$HOME/.aspnet/dev-certs/trust:/usr/lib/ssl/certs"`)

**Linux — si .NET está instalado vía gestor de paquetes de la distro** (no vía script manual): la variable `DOTNET_ROOT` debe apuntar a la ruta real del SDK (ej. `/usr/lib/dotnet`, confirmar con `which dotnet`), no a `~/.dotnet`. Si `dotnet-ef` u otras herramientas globales fallan con "You must install .NET to run this application", revisar `DOTNET_ROOT` en `~/.bashrc`.

## 11. Notas de mantenimiento

El desarrollador documenta el proceso completo con fines de aprendizaje. La intención es que el proyecto quede documentado para que, en el futuro, otra persona pueda mantenerlo si es necesario.
