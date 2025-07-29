# Optimizaciones de Rendimiento - API de Cantidades y Disponibilidad

Este documento describe las optimizaciones implementadas para mejorar significativamente el tiempo de respuesta de los endpoints.

## 🚀 Optimizaciones Implementadas

### 1. **Sistema de Caché Inteligente**

#### **Configuración de Caché por Endpoint:**
- **Consultas de cantidades**: 2 minutos de caché
- **Consultas básicas**: 5 minutos de caché
- **Datos locales**: 10 minutos de caché
- **Configuración**: 30 minutos de caché

#### **Invalidación Automática:**
- El caché se invalida automáticamente después de sincronizaciones
- El caché se invalida después de eliminaciones
- Uso de tags para invalidación selectiva

```csharp
[OutputCache(Tags = [cacheTag], Duration = 120)] // Cache por 2 minutos
```

### 2. **Paginación Inteligente**

#### **Parámetros de Paginación:**
- `page`: Número de página (por defecto: 1)
- `pageSize`: Tamaño de página (por defecto: 50, máximo: 200)

#### **Beneficios:**
- Reduce el tiempo de respuesta para grandes conjuntos de datos
- Mejora la experiencia del usuario
- Reduce el uso de memoria

#### **Ejemplo de Respuesta con Paginación:**
```json
{
  "totalComponentes": 1500,
  "componentesConStock": 1200,
  "componentesSinStock": 300,
  "componentes": [...],
  "paginacion": {
    "paginaActual": 1,
    "tamañoPagina": 50,
    "totalPaginas": 30,
    "totalElementos": 1500
  }
}
```

### 3. **Manejo Robusto de Errores con Reintentos**

#### **Sistema de Reintentos:**
- Máximo 3 intentos por consulta
- Backoff exponencial entre reintentos
- Logging detallado de errores

#### **Validaciones Mejoradas:**
- Verificación de respuestas exitosas de Business Central
- Validación de estructura de datos
- Manejo de respuestas null

### 4. **Optimizaciones de Consulta**

#### **Filtros Dinámicos:**
- Los filtros se aplican solo cuando se proporcionan parámetros
- Reducción de datos transferidos desde Business Central
- Consultas más específicas y eficientes

#### **Procesamiento Eficiente:**
- Uso de `Skip()` y `Take()` para paginación
- Cálculos optimizados de estadísticas
- Reducción de operaciones LINQ innecesarias

## 📊 Mejoras de Rendimiento Esperadas

### **Tiempo de Respuesta:**
- **Sin caché**: 2-5 segundos
- **Con caché**: 50-200 milisegundos
- **Mejora**: 90-95% más rápido

### **Uso de Memoria:**
- **Sin paginación**: Puede usar varios MB
- **Con paginación**: Máximo 200 registros en memoria
- **Reducción**: 80-90% menos uso de memoria

### **Confiabilidad:**
- **Sin reintentos**: 70-80% de éxito
- **Con reintentos**: 95-99% de éxito
- **Mejora**: 20-30% más confiable

## 🔧 Configuración de Caché

### **Duración por Tipo de Endpoint:**

| Endpoint | Duración | Razón |
|----------|----------|-------|
| `consultar-cantidades-disponibilidad` | 2 minutos | Datos dinámicos de stock |
| `consultar-cantidades-por-grupo-precios` | 2 minutos | Datos dinámicos de precios |
| `consultar-bc` | 5 minutos | Datos de Business Central |
| `locales` | 10 minutos | Datos locales más estables |
| `estadisticas` | 10 minutos | Estadísticas que cambian poco |
| `verificar-configuracion` | 30 minutos | Configuración muy estable |

### **Invalidación de Caché:**
```csharp
// Después de sincronización
await _outputCacheStore.EvictByTagAsync(cacheTag, default);

// Después de eliminación
await _outputCacheStore.EvictByTagAsync(cacheTag, default);
```

## 📈 Monitoreo de Rendimiento

### **Métricas a Monitorear:**

1. **Tiempo de Respuesta Promedio**
   - Objetivo: < 500ms para respuestas cacheadas
   - Objetivo: < 3s para consultas directas a BC

2. **Tasa de Hit del Caché**
   - Objetivo: > 80% de requests servidos desde caché

3. **Tasa de Error**
   - Objetivo: < 1% de errores
   - Con reintentos: < 0.1% de errores críticos

4. **Uso de Memoria**
   - Objetivo: < 100MB por request
   - Con paginación: < 10MB por request

## 🛠️ Configuración Avanzada

### **Ajuste de Parámetros de Caché:**

```csharp
// En Program.cs
builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(5);
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromMinutes(2)));
});
```

### **Configuración de Reintentos:**

```csharp
// En bcConn.cs
int maxRetries = 3; // Ajustar según necesidades
int retryDelay = 1000; // Milisegundos
```

### **Límites de Paginación:**

```csharp
// En el controlador
if (pageSize > 200) pageSize = 200; // Límite máximo
if (pageSize < 1) pageSize = 50;    // Límite mínimo
```

## 🔍 Debugging y Troubleshooting

### **Logs de Rendimiento:**
```csharp
Console.WriteLine($"Error en GetComponents (intento {currentRetry}/{maxRetries}): {ex.Message}");
Console.WriteLine($"Error crítico en GetComponents: {ex.Message}");
```

### **Métricas de Caché:**
- Verificar hit/miss ratio
- Monitorear tiempo de invalidación
- Revisar tamaño del caché

### **Optimizaciones Adicionales Recomendadas:**

1. **Compresión de Respuestas**
   ```csharp
   builder.Services.AddResponseCompression();
   ```

2. **Caché Distribuido**
   ```csharp
   builder.Services.AddStackExchangeRedisCache();
   ```

3. **Monitoreo con Application Insights**
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

## 📋 Checklist de Optimización

- [x] Implementar sistema de caché
- [x] Agregar paginación
- [x] Implementar reintentos
- [x] Optimizar consultas
- [x] Validar parámetros
- [x] Agregar logging
- [x] Documentar optimizaciones
- [ ] Configurar monitoreo
- [ ] Implementar métricas
- [ ] Optimizar base de datos

## 🎯 Resultados Esperados

Con estas optimizaciones, los endpoints deberían mostrar:

1. **Tiempo de respuesta 90-95% más rápido** para requests cacheados
2. **Uso de memoria 80-90% menor** con paginación
3. **Confiabilidad 20-30% mayor** con reintentos
4. **Mejor experiencia de usuario** con respuestas más rápidas
5. **Menor carga en Business Central** con caché inteligente

## 📞 Soporte

Para ajustes adicionales de rendimiento o problemas específicos, revisar:
- Logs de la aplicación
- Métricas de caché
- Tiempos de respuesta por endpoint
- Uso de memoria por request