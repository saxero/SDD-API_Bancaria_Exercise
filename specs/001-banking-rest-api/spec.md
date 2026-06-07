# Feature Specification: Banking REST API Mínima

**Feature Branch**: `feature/001-banking-rest-api`

**Created**: 2026-06-07

**Status**: Draft

**Input**: User description: "Banking REST API minimalista con consulta de saldo y transferencias"

## User Scenarios & Testing

### User Story 1 - Consultar saldo de una cuenta (Priority: P1)

Como usuario del sistema, quiero consultar el saldo actual de una cuenta específica para conocer el dinero disponible.

**Why this priority**: Es la operación más simple y fundamental; cualquier interacción con el sistema requiere conocer saldos.

**Independent Test**: Puede probarse independientemente consultando el saldo de cualquiera de las 3 cuentas precargadas.

**Acceptance Scenarios**:

1. **Given** una cuenta existente con saldo conocido (ACC-001 con $1000), **When** se consulta su saldo, **Then** el sistema retorna el saldo actual correcto.
2. **Given** una cuenta que no existe (ej. ACC-999), **When** se consulta su saldo, **Then** el sistema retorna un error indicando que la cuenta no existe.
3. **Given** una cuenta con saldo cero (ACC-003 con $0), **When** se consulta su saldo, **Then** el sistema retorna $0 correctamente.

---

### User Story 2 - Transferir dinero entre cuentas (Priority: P1)

Como usuario del sistema, quiero transferir dinero desde mi cuenta origen a una cuenta destino para mover fondos entre cuentas.

**Why this priority**: Es la operación principal de valor del sistema; sin transferencias no hay funcionalidad bancaria.

**Independent Test**: Puede probarse independientemente transfiriendo fondos entre las cuentas precargadas y verificando los saldos resultantes.

**Acceptance Scenarios**:

1. **Given** la cuenta ACC-001 con $1000 y ACC-002 con $500, **When** se transfieren $300 de ACC-001 a ACC-002, **Then** ACC-001 queda con $700 y ACC-002 con $800.
2. **Given** la cuenta ACC-001 con $1000, **When** se intenta transferir $2000 de ACC-001 a ACC-002, **Then** el sistema rechaza la transferencia por saldo insuficiente.
3. **Given** la cuenta ACC-001, **When** se intenta transferir $-50 de ACC-001 a ACC-002, **Then** el sistema rechaza la transferencia por monto inválido.
4. **Given** la cuenta ACC-001, **When** se intenta transferir $0 de ACC-001 a ACC-002, **Then** el sistema rechaza la transferencia por monto inválido.
5. **Given** la cuenta ACC-001, **When** se intenta transferir $100 de ACC-001 a ACC-001, **Then** el sistema rechaza la transferencia por ser la misma cuenta.

---

### Edge Cases

- ¿Qué ocurre cuando se consulta una cuenta con ID vacío o nulo? El sistema debe retornar un error de solicitud inválida.
- ¿Qué ocurre cuando se omite un campo requerido en la transferencia (cuenta origen, cuenta destino, monto)? El sistema debe retornar un error de solicitud inválida.
- ¿Qué ocurre cuando el formato del ID de cuenta no es válido? El sistema debe retornar un error indicando formato inválido.
- ¿Qué ocurre cuando se transfiere un monto que deja la cuenta origen exactamente en cero? La transferencia debe permitirse y la cuenta origen queda con $0.

## Requirements

### Functional Requirements

- **FR-001**: El sistema DEBE permitir consultar el saldo de una cuenta específica mediante su identificador único.
- **FR-002**: El sistema DEBE retornar el saldo actual de la cuenta en formato numérico.
- **FR-003**: El sistema DEBE permitir transferir dinero entre dos cuentas especificando origen, destino y monto.
- **FR-004**: El sistema DEBE validar que la cuenta origen exista antes de procesar una transferencia.
- **FR-005**: El sistema DEBE validar que la cuenta destino exista antes de procesar una transferencia.
- **FR-006**: El sistema DEBE rechazar transferencias donde el monto sea menor o igual a cero.
- **FR-007**: El sistema DEBE rechazar transferencias donde la cuenta origen y destino sean la misma.
- **FR-008**: El sistema DEBE rechazar transferencias donde la cuenta origen no tenga saldo suficiente para cubrir el monto.
- **FR-009**: El sistema DEBE actualizar los saldos de ambas cuentas (origen y destino) atómicamente al completar una transferencia exitosa.
- **FR-010**: El sistema DEBE retornar un mensaje de error en formato JSON `{ "error": "mensaje" }` para cualquier condición de error.

### Key Entities

- **Account (Cuenta)**: Representa una cuenta bancaria con un identificador único (accountId) y un saldo actual (balance). Datos semilla: ACC-001 ($1000), ACC-002 ($500), ACC-003 ($0).
- **Transfer (Transferencia)**: Representa una operación de transferencia con cuenta origen (source), cuenta destino (target) y monto (amount). No se persiste; solo modifica los saldos de las cuentas involucradas.

## Success Criteria

### Measurable Outcomes

- **SC-001**: Un usuario puede consultar el saldo de cualquier cuenta existente en menos de 1 segundo.
- **SC-002**: Un usuario puede completar una transferencia exitosa entre dos cuentas en menos de 2 segundos.
- **SC-003**: El sistema rechaza correctamente el 100% de las transferencias que violan las reglas de negocio (saldo insuficiente, monto inválido, misma cuenta).
- **SC-004**: El sistema mantiene la consistencia de saldos: el saldo total entre todas las cuentas se conserva después de cada transferencia exitosa.

## Assumptions

- El sistema funciona como API REST stateless: cada solicitud es independiente y no requiere sesión de usuario.
- No existe autenticación ni autorización: cualquier cliente puede consultar saldos y realizar transferencias.
- Los datos se almacenan únicamente en memoria volátil: al reiniciar el sistema, los saldos vuelven al estado semilla.
- El alcance se limita estrictamente a las 2 operaciones especificadas: no hay historial de transacciones, notificaciones, ni cancelaciones.
- Los ID de cuenta son strings alfanuméricos proporcionados por el cliente (ej. ACC-001).
- El sistema opera con una moneda única sin tipo de cambio (dólar estadounidense).
