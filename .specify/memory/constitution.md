<!--
  Sync Impact Report:
  - Version change: 0.0.0 → 1.0.0 (initial)
  - New constitution created from scratch for SDD API Bancaria
  - Sections added: Core Principles (6 principios), Estándares Técnicos, Definition of Done
  - Templates requiring updates: ✅ plan-template.md (pending review), ✅ spec-template.md (pending review), ✅ tasks-template.md (pending review)
  - No placeholders deferred
-->
# SDD API Bancaria — Constitución

## Core Principles

### I. Simplicidad ante todo (KISS)
El código funcional y correcto es mejor que el código arquitectónicamente perfecto.
- NO se usarán interfaces, abstracciones o patrones a menos que sean estrictamente
  necesarios para resolver un problema concreto.
- Los servicios serán clases concretas directamente. Sin capas de abstracción
  innecesarias.
- Se prefiere consolidar en menos archivos con más contenido antes que
  fragmentar en múltiples archivos pequeños.
- El código DEBE compilar y ejecutarse en el primer intento sin configuración
  adicional.

### II. Estructura del proyecto
Todo el código fuente DEBE ubicarse bajo `src/BankingApi/` en la raíz del repositorio.
- `src/BankingApi/` — Proyecto principal de la Web API (controladores, servicios,
  modelos, etc.)
- `src/BankingApi.Tests/` — Proyecto de pruebas unitarias (xUnit)
- No se permiten proyectos separados por capa (Domain, Application, Infrastructure)
  a menos que se demuestre una necesidad explícita.

### III. Seguridad mínima (solo lógica de negocio)
La API es exclusivamente para laboratorio. Aplica las siguientes reglas:
- SIN autenticación (no hay login, tokens, ni credenciales)
- SIN autorización (no hay roles, permisos ni políticas)
- SIN HTTPS (solo HTTP)
- Sin middleware de seguridad de ningún tipo
- Sin logging de seguridad ni auditoría
- Solo existe la lógica de negocio bancaria y sus validaciones de dominio

### IV. Validaciones de dominio
Toda regla de negocio DEBE validarse en el dominio, no en la capa de
presentación. Reglas críticas mínimas:
- No transferir saldo superior al disponible (saldo insuficiente)
- No transferir montos negativos o cero
- No permitir cuentas con saldo negativo en apertura
- Las cuentas DEben identificarse de forma única
- Las transferencias DEBEN registrar la fecha de operación

### V. Pruebas unitarias esenciales
Cada regla de negocio crítica DEBE tener al menos una prueba unitaria.
- Framework: xUnit (el estándar de facto para .NET)
- Sin mocks complejos. Preferir datos reales o stubs inline.
- Las pruebas DEBEN ser deterministas y aisladas.
- NO se requieren pruebas de integración, solo unitarias.

### VI. Código limpio (estándares C# mínimos)
- Nombres descriptivos en inglés para clases, métodos, propiedades y endpoints.
- XML comments opcional; el código DEBE ser autoexplicativo.
- Código muerto (variables no usadas, métodos privados no llamados) NO está
  permitido.
- Seguir convenciones de nomenclatura estándar de C#: PascalCase para clases y
  métodos, camelCase para parámetros y variables locales.

## VII. Test-Driven Development

- All business logic must be implemented using TDD.
- Tests are written before production code.
- Follow Red-Green-Refactor cycle.
- No implementation task may start until a failing test exists.

## Estándares técnicos

- **Runtime**: .NET 8.0+
- **Framework Web**: ASP.NET Core minimal API o controllers (el más simple que
  funcione)
- **Pruebas**: xUnit + FluentAssertions (opcional)
- **Documentación**: Swagger (Swashbuckle) habilitado por defecto
- **Base de datos**: Ninguna. Los datos se mantienen en memoria (diccionarios,
  listas estáticas) para simplificar el laboratorio.
- **Formato de respuesta**: JSON (por defecto en ASP.NET Core)
- **Códigos HTTP**: 200 OK, 201 Created, 400 Bad Request, 404 Not Found.
  Sin códigos de seguridad (401, 403).

## Definition of Done (DoD)

Una historia/feature se considera completa cuando:

1. **Compila** — `dotnet build` sin errores ni warnings.
2. **Ejecuta** — `dotnet run` inicia el servidor sin excepciones.
3. **Responde correctamente** — Los endpoints devuelven los códigos HTTP y
   cuerpos de respuesta esperados.
4. **Swagger accesible** — La UI de Swagger está disponible en `/swagger`.
5. **Pruebas pasan** — `dotnet test` ejecuta todas las pruebas unitarias sin
   fallos.

## Governance

- Esta constitución es la autoridad máxima sobre decisiones de arquitectura y
  diseño en este proyecto.
- Cualquier desviación de estos principios DEBE ser documentada y aprobada
  explícitamente.
- Las amendments (enmiendas) requieren actualizar este documento, incrementar
  la versión según semver y notificar al equipo.
- El AGENTS.md en la raíz del proyecto contiene las instrucciones operativas
  para el agente de IA; esta constitución prevalece en caso de conflicto.

**Version**: 1.0.0 | **Ratified**: 2026-06-07 | **Last Amended**: 2026-06-07
