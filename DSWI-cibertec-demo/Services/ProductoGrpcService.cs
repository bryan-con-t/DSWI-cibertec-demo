using Grpc.Core;
using DSWI_cibertec_demo.Grpc;
using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Models;
using Microsoft.Extensions.Logging;

namespace DSWI_cibertec_demo.Services
{
    public class ProductoGrpcService : ProductoService.ProductoServiceBase
    {
        private readonly ProductoRepository _repository;
        private readonly ILogger<ProductoGrpcService> _logger;

        public ProductoGrpcService(ProductoRepository repository, ILogger<ProductoGrpcService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public override async Task<ProductoResponse> ObtenerProducto(ProductoRequest request, ServerCallContext context)
        {
            try
            {
                var productos = await _repository.ObtenerProductosAsync();
                var producto = productos.FirstOrDefault(p => p.Id == request.Id);

                if (producto == null)
                {
                    return new ProductoResponse
                    {
                        Mensaje = $"Producto con ID {request.Id} no encontrado"
                    };
                }

                return new ProductoResponse
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = (double)producto.Precio,
                    Cantidad = producto.Cantidad,
                    Estado = producto.Estado,
                    Mensaje = "Producto encontrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID {ProductId}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Error interno del servidor"));
            }
        }

        public override async Task<ProductoResponse> CrearProducto(ProductoCrearRequest request, ServerCallContext context)
        {
            try
            {
                var nuevoProducto = new ProductoModel
                {
                    Nombre = request.Nombre,
                    Precio = (decimal)request.Precio,
                    Cantidad = request.Cantidad,
                    Estado = request.Estado
                };

                await _repository.AgregarProductoAsync(nuevoProducto);

                // Para obtener el ID, necesitaríamos buscar el último producto agregado
                var productos = await _repository.ObtenerProductosAsync();
                var productoRecienCreado = productos.OrderByDescending(p => p.Id).First();

                return new ProductoResponse
                {
                    Id = productoRecienCreado.Id,
                    Nombre = request.Nombre,
                    Precio = request.Precio,
                    Cantidad = request.Cantidad,
                    Estado = request.Estado,
                    Mensaje = "Producto creado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {ProductName}", request.Nombre);
                throw new RpcException(new Status(StatusCode.Internal, "Error al crear producto"));
            }
        }

        public override async Task<ListarProductosResponse> ListarProductos(ListarRequest request, ServerCallContext context)
        {
            try
            {
                if (request.Page > 0 && request.PageSize > 0)
                {
                    // Usar paginación
                    var (productos, totalRegistros) = await _repository.ObtenerProductosPaginadoAsync(request.Page, request.PageSize);
                    var totalPaginas = (int)Math.Ceiling((double)totalRegistros / request.PageSize);

                    var response = new ListarProductosResponse
                    {
                        TotalRegistros = totalRegistros,
                        PaginaActual = request.Page,
                        TotalPaginas = totalPaginas
                    };

                    foreach (var producto in productos)
                    {
                        response.Productos.Add(new ProductoResponse
                        {
                            Id = producto.Id,
                            Nombre = producto.Nombre,
                            Precio = (double)producto.Precio,
                            Cantidad = producto.Cantidad,
                            Estado = producto.Estado
                        });
                    }

                    return response;
                }
                else
                {
                    // Listar todos
                    var productos = await _repository.ObtenerProductosAsync();
                    var response = new ListarProductosResponse
                    {
                        TotalRegistros = productos.Count,
                        PaginaActual = 1,
                        TotalPaginas = 1
                    };

                    foreach (var producto in productos)
                    {
                        response.Productos.Add(new ProductoResponse
                        {
                            Id = producto.Id,
                            Nombre = producto.Nombre,
                            Precio = (double)producto.Precio,
                            Cantidad = producto.Cantidad,
                            Estado = producto.Estado
                        });
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                throw new RpcException(new Status(StatusCode.Internal, "Error al listar productos"));
            }
        }

        public override async Task<ProductoResponse> ActualizarProducto(ProductoActualizarRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Método no implementado"));
        }

        public override async Task<EliminarResponse> EliminarProducto(ProductoRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Método no implementado"));
        }
    }
}