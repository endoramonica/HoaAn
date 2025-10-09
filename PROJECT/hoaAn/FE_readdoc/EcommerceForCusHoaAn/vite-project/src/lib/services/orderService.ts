/**
 * Order Service
 * Xử lý tạo đơn hàng, theo dõi đơn hàng, lịch sử đơn hàng
 */

import { apiRequest, buildQueryString } from '../api/client';
import type {
  OrderDto,
  CreateOrderRequest,
  OrderFilterParams,
  PagedResponse,
  OrderStatus,
  PaymentStatus,
  PaymentMethod,
} from '../api/types';

/**
 * Mock data cho development
 */
const generateMockOrders = (count: number = 10): OrderDto[] => {
  const orders: OrderDto[] = [];
  const statuses: OrderStatus[] = [
    'Pending' as OrderStatus,
    'Confirmed' as OrderStatus,
    'Processing' as OrderStatus,
    'Shipping' as OrderStatus,
    'Delivered' as OrderStatus,
  ];

  for (let i = 1; i <= count; i++) {
    const total = Math.floor(Math.random() * 2000000) + 100000;
    const shippingFee = 30000;
    const discount = Math.floor(Math.random() * 100000);
    const subtotal = total - shippingFee + discount;

    orders.push({
      id: `order-${i}`,
      orderNumber: `ORD${String(i).padStart(6, '0')}`,
      userId: 'user-123',
      user: {
        id: 'user-123',
        email: 'test@example.com',
        fullName: 'Nguyễn Văn A',
        phoneNumber: '0901234567',
        role: 'Customer' as any,
        isEmailConfirmed: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
      status: statuses[i % statuses.length],
      paymentStatus: i % 3 === 0 ? ('Paid' as PaymentStatus) : ('Pending' as PaymentStatus),
      paymentMethod: 'COD' as PaymentMethod,
      items: [
        {
          id: `item-${i}-1`,
          productId: `product-${i}`,
          product: {
            id: `product-${i}`,
            name: `Sản phẩm ${i}`,
            slug: `san-pham-${i}`,
            description: 'Mô tả sản phẩm',
            price: 150000,
            categoryId: 'cat-1',
            category: {
              id: 'cat-1',
              name: 'Hương & Nến',
              slug: 'huong-nen',
              sortOrder: 1,
              isActive: true,
            },
            images: [],
            stock: 100,
            isActive: true,
            isFeatured: false,
            rating: 4.5,
            reviewCount: 10,
            tags: [],
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          },
          quantity: 2,
          price: 150000,
          total: 300000,
        },
      ],
      subtotal,
      shippingFee,
      discount,
      total,
      shippingAddress: {
        fullName: 'Nguyễn Văn A',
        phoneNumber: '0901234567',
        addressLine1: '123 Nguyễn Huệ',
        ward: 'Phường Bến Nghé',
        district: 'Quận 1',
        province: 'TP. Hồ Chí Minh',
      },
      trackingNumber: i % 2 === 0 ? `TRACK${String(i).padStart(8, '0')}` : undefined,
      createdAt: new Date(Date.now() - i * 86400000).toISOString(),
      updatedAt: new Date(Date.now() - i * 3600000).toISOString(),
      completedAt: i % 5 === 0 ? new Date(Date.now() - i * 43200000).toISOString() : undefined,
    });
  }

  return orders;
};

const MOCK_ORDERS = generateMockOrders(20);

const USE_MOCK = process.env.REACT_APP_USE_MOCK === 'true';

class OrderService {
  /**
   * Lấy danh sách đơn hàng với filter, pagination
   */
  async getOrders(params?: OrderFilterParams): Promise<PagedResponse<OrderDto>> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 800));

        // Apply filters
        let filtered = [...MOCK_ORDERS];

        if (params?.status) {
          filtered = filtered.filter(o => o.status === params.status);
        }

        if (params?.paymentStatus) {
          filtered = filtered.filter(o => o.paymentStatus === params.paymentStatus);
        }

        if (params?.search) {
          const search = params.search.toLowerCase();
          filtered = filtered.filter(o =>
            o.orderNumber.toLowerCase().includes(search) ||
            o.trackingNumber?.toLowerCase().includes(search)
          );
        }

        if (params?.fromDate) {
          filtered = filtered.filter(o =>
            new Date(o.createdAt) >= new Date(params.fromDate!)
          );
        }

        if (params?.toDate) {
          filtered = filtered.filter(o =>
            new Date(o.createdAt) <= new Date(params.toDate!)
          );
        }

        // Apply pagination
        const pageNumber = params?.pageNumber || 1;
        const pageSize = params?.pageSize || 10;
        const totalCount = filtered.length;
        const totalPages = Math.ceil(totalCount / pageSize);
        const startIndex = (pageNumber - 1) * pageSize;
        const items = filtered.slice(startIndex, startIndex + pageSize);

        return {
          items,
          pageNumber,
          pageSize,
          totalPages,
          totalCount,
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < totalPages,
        };
      }

      const queryString = buildQueryString(params || {});
      return await apiRequest.get<PagedResponse<OrderDto>>(`/orders${queryString}`);
    } catch (error) {
      console.error('Get orders error:', error);
      throw error;
    }
  }

  /**
   * Lấy chi tiết đơn hàng theo ID
   */
  async getOrderById(id: string): Promise<OrderDto> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const order = MOCK_ORDERS.find(o => o.id === id);
        if (!order) {
          throw new Error('Không tìm thấy đơn hàng');
        }
        return order;
      }

      return await apiRequest.get<OrderDto>(`/orders/${id}`);
    } catch (error) {
      console.error('Get order by id error:', error);
      throw error;
    }
  }

  /**
   * Lấy chi tiết đơn hàng theo order number
   */
  async getOrderByNumber(orderNumber: string): Promise<OrderDto> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const order = MOCK_ORDERS.find(o => o.orderNumber === orderNumber);
        if (!order) {
          throw new Error('Không tìm thấy đơn hàng');
        }
        return order;
      }

      return await apiRequest.get<OrderDto>(`/orders/number/${orderNumber}`);
    } catch (error) {
      console.error('Get order by number error:', error);
      throw error;
    }
  }

  /**
   * Tạo đơn hàng mới
   */
  async createOrder(request: CreateOrderRequest): Promise<OrderDto> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1500));

        // Validation
        if (!request.items || request.items.length === 0) {
          throw new Error('Đơn hàng phải có ít nhất 1 sản phẩm');
        }

        if (!request.shippingAddress) {
          throw new Error('Vui lòng cung cấp địa chỉ giao hàng');
        }

        // Calculate totals (mock)
        const subtotal = request.items.reduce((sum, item) => sum + (item.quantity * 150000), 0);
        const shippingFee = 30000;
        const discount = 0;
        const total = subtotal + shippingFee - discount;

        const newOrder: OrderDto = {
          id: `order-new-${Date.now()}`,
          orderNumber: `ORD${String(Date.now()).slice(-6)}`,
          userId: 'user-123',
          user: {
            id: 'user-123',
            email: 'test@example.com',
            fullName: 'Nguyễn Văn A',
            phoneNumber: '0901234567',
            role: 'Customer' as any,
            isEmailConfirmed: true,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          },
          status: 'Pending' as OrderStatus,
          paymentStatus: 'Pending' as PaymentStatus,
          paymentMethod: request.paymentMethod,
          items: request.items.map((item, index) => ({
            id: `item-new-${index}`,
            productId: item.productId,
            product: {
              id: item.productId,
              name: 'Sản phẩm mẫu',
              slug: 'san-pham-mau',
              description: 'Mô tả',
              price: 150000,
              categoryId: 'cat-1',
              category: {
                id: 'cat-1',
                name: 'Danh mục',
                slug: 'danh-muc',
                sortOrder: 1,
                isActive: true,
              },
              images: [],
              stock: 100,
              isActive: true,
              isFeatured: false,
              rating: 4.5,
              reviewCount: 10,
              tags: [],
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString(),
            },
            quantity: item.quantity,
            price: 150000,
            total: item.quantity * 150000,
          })),
          subtotal,
          shippingFee,
          discount,
          total,
          shippingAddress: request.shippingAddress,
          billingAddress: request.billingAddress,
          note: request.note,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };

        return newOrder;
      }

      return await apiRequest.post<OrderDto>('/orders', request);
    } catch (error) {
      console.error('Create order error:', error);
      throw error;
    }
  }

  /**
   * Hủy đơn hàng
   */
  async cancelOrder(id: string, reason?: string): Promise<OrderDto> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const order = MOCK_ORDERS.find(o => o.id === id);
        if (!order) {
          throw new Error('Không tìm thấy đơn hàng');
        }

        return {
          ...order,
          status: 'Cancelled' as OrderStatus,
          updatedAt: new Date().toISOString(),
        };
      }

      return await apiRequest.post<OrderDto>(`/orders/${id}/cancel`, { reason });
    } catch (error) {
      console.error('Cancel order error:', error);
      throw error;
    }
  }

  /**
   * Theo dõi đơn hàng theo tracking number
   */
  async trackOrder(trackingNumber: string): Promise<{
    order: OrderDto;
    timeline: Array<{
      status: string;
      timestamp: string;
      description: string;
      location?: string;
    }>;
  }> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const order = MOCK_ORDERS.find(o => o.trackingNumber === trackingNumber);
        if (!order) {
          throw new Error('Không tìm thấy đơn hàng với mã vận đơn này');
        }

        // Mock timeline
        const timeline = [
          {
            status: 'Pending',
            timestamp: order.createdAt,
            description: 'Đơn hàng đã được tạo',
          },
          {
            status: 'Confirmed',
            timestamp: new Date(new Date(order.createdAt).getTime() + 3600000).toISOString(),
            description: 'Đơn hàng đã được xác nhận',
          },
          {
            status: 'Processing',
            timestamp: new Date(new Date(order.createdAt).getTime() + 7200000).toISOString(),
            description: 'Đang chuẩn bị hàng',
          },
        ];

        if (order.status === 'Shipping' || order.status === 'Delivered') {
          timeline.push({
            status: 'Shipping',
            timestamp: new Date(new Date(order.createdAt).getTime() + 86400000).toISOString(),
            description: 'Đơn hàng đang được giao',
            location: 'TP. Hồ Chí Minh',
          });
        }

        if (order.status === 'Delivered') {
          timeline.push({
            status: 'Delivered',
            timestamp: order.completedAt || new Date().toISOString(),
            description: 'Đơn hàng đã được giao thành công',
          });
        }

        return { order, timeline };
      }

      return await apiRequest.get(`/orders/track/${trackingNumber}`);
    } catch (error) {
      console.error('Track order error:', error);
      throw error;
    }
  }

  /**
   * Lấy lịch sử đơn hàng của user hiện tại
   */
  async getMyOrders(params?: OrderFilterParams): Promise<PagedResponse<OrderDto>> {
    try {
      if (USE_MOCK) {
        // Reuse getOrders với mock data
        return this.getOrders(params);
      }

      const queryString = buildQueryString(params || {});
      return await apiRequest.get<PagedResponse<OrderDto>>(`/orders/my${queryString}`);
    } catch (error) {
      console.error('Get my orders error:', error);
      throw error;
    }
  }

  /**
   * Đánh giá đơn hàng
   */
  async rateOrder(orderId: string, rating: number, review?: string): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (rating < 1 || rating > 5) {
          throw new Error('Đánh giá phải từ 1 đến 5 sao');
        }
        
        return;
      }

      await apiRequest.post(`/orders/${orderId}/rate`, { rating, review });
    } catch (error) {
      console.error('Rate order error:', error);
      throw error;
    }
  }

  /**
   * Yêu cầu hoàn tiền
   */
  async requestRefund(orderId: string, reason: string): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (!reason) {
          throw new Error('Vui lòng cung cấp lý do hoàn tiền');
        }
        
        return;
      }

      await apiRequest.post(`/orders/${orderId}/refund`, { reason });
    } catch (error) {
      console.error('Request refund error:', error);
      throw error;
    }
  }

  /**
   * Cập nhật trạng thái đơn hàng (Admin)
   */
  async updateOrderStatus(orderId: string, status: OrderStatus): Promise<OrderDto> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const order = MOCK_ORDERS.find(o => o.id === orderId);
        if (!order) {
          throw new Error('Không tìm thấy đơn hàng');
        }

        return {
          ...order,
          status,
          updatedAt: new Date().toISOString(),
        };
      }

      return await apiRequest.patch<OrderDto>(`/orders/${orderId}/status`, { status });
    } catch (error) {
      console.error('Update order status error:', error);
      throw error;
    }
  }
}

export const orderService = new OrderService();
export default orderService;
