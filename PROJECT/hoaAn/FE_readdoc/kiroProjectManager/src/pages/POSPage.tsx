import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { useProducts } from '../lib/hooks/useProducts';
import { useCreateOrder } from '../lib/hooks/useOrders';
import { toast } from 'sonner';
import { Search, Plus, Minus, Trash2, ShoppingCart } from 'lucide-react';

interface CartItem {
    id: string;
    name: string;
    price: number;
    quantity: number;
}

export const POSPage = () => {
    const [search, setSearch] = useState('');
    const [cart, setCart] = useState<CartItem[]>([]);
    const [checkoutOpen, setCheckoutOpen] = useState(false);
    const [customerPaid, setCustomerPaid] = useState('');

    const { data: productsData } = useProducts({ SearchTerm: search });
    const createOrder = useCreateOrder();

    // Extract products from paginated response
    // Response structure: productsData.data.data.data.items
    const products = (productsData?.data?.data as any)?.data?.items || [];

    // Debug log
    console.log('POSPage - productsData:', productsData);
    console.log('POSPage - products:', products);

    const addToCart = (product: any) => {
        const existing = cart.find(item => item.id === product.id);
        if (existing) {
            setCart(cart.map(item =>
                item.id === product.id
                    ? { ...item, quantity: item.quantity + 1 }
                    : item
            ));
        } else {
            setCart([...cart, {
                id: product.id,
                name: product.name,
                price: product.price,
                quantity: 1,
            }]);
        }
        toast.success(`Đã thêm ${product.name} vào giỏ`);
    };

    const updateQuantity = (id: string, delta: number) => {
        setCart(cart.map(item => {
            if (item.id === id) {
                const newQty = item.quantity + delta;
                return newQty > 0 ? { ...item, quantity: newQty } : item;
            }
            return item;
        }).filter(item => item.quantity > 0));
    };

    const removeFromCart = (id: string) => {
        setCart(cart.filter(item => item.id !== id));
    };

    const total = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    const quickCash = [1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000];

    const handleCheckout = () => {
        const paid = parseInt(customerPaid) || 0;
        if (paid < total) {
            toast.error('Số tiền khách đưa không đủ');
            return;
        }

        createOrder.mutate(
            {
                items: cart,
                totalAmount: total,
                paidAmount: paid,
            },
            {
                onSuccess: () => {
                    toast.success('Thanh toán thành công!');
                    setCart([]);
                    setCheckoutOpen(false);
                    setCustomerPaid('');
                },
            }
        );
    };

    return (
        <div className="space-y-6">
            <h1 className="text-2xl font-bold text-gray-900">
                Bán Hàng (POS)
            </h1>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Products */}
                <div className="lg:col-span-2">
                    <Card>
                        <Input
                            placeholder="Tìm kiếm sản phẩm..."
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            icon={<Search className="w-5 h-5" />}
                            className="mb-4"
                        />

                        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                            {products.map((product: any) => (
                                <button
                                    key={product.id}
                                    onClick={() => addToCart(product)}
                                    className="p-4 border border-gray-200 rounded-lg hover:border-blue-500:border-blue-400 transition-colors text-left"
                                >
                                    <h3 className="font-medium text-gray-900 mb-1">
                                        {product.name}
                                    </h3>
                                    <p className="text-blue-600 font-semibold">
                                        {formatCurrency(product.price)}
                                    </p>
                                </button>
                            ))}
                        </div>
                    </Card>
                </div>

                {/* Cart */}
                <div>
                    <Card title="Giỏ Hàng">
                        <div className="space-y-3">
                            {cart.length === 0 ? (
                                <p className="text-center text-gray-500 py-8">
                                    Giỏ hàng trống
                                </p>
                            ) : (
                                <>
                                    {cart.map((item) => (
                                        <div
                                            key={item.id}
                                            className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                                        >
                                            <div className="flex-1">
                                                <p className="font-medium text-gray-900">
                                                    {item.name}
                                                </p>
                                                <p className="text-sm text-gray-600">
                                                    {formatCurrency(item.price)}
                                                </p>
                                            </div>
                                            <div className="flex items-center gap-2">
                                                <button
                                                    onClick={() => updateQuantity(item.id, -1)}
                                                    className="p-1 hover:bg-gray-200:bg-gray-600 rounded"
                                                >
                                                    <Minus className="w-4 h-4" />
                                                </button>
                                                <span className="w-8 text-center font-medium">
                                                    {item.quantity}
                                                </span>
                                                <button
                                                    onClick={() => updateQuantity(item.id, 1)}
                                                    className="p-1 hover:bg-gray-200:bg-gray-600 rounded"
                                                >
                                                    <Plus className="w-4 h-4" />
                                                </button>
                                                <button
                                                    onClick={() => removeFromCart(item.id)}
                                                    className="p-1 hover:bg-red-100:bg-red-900 text-red-600 rounded"
                                                >
                                                    <Trash2 className="w-4 h-4" />
                                                </button>
                                            </div>
                                        </div>
                                    ))}

                                    <div className="pt-3 border-t border-gray-200">
                                        <div className="flex justify-between items-center mb-4">
                                            <span className="text-lg font-semibold text-gray-900">
                                                Tổng cộng:
                                            </span>
                                            <span className="text-xl font-bold text-blue-600">
                                                {formatCurrency(total)}
                                            </span>
                                        </div>
                                        <Button
                                            variant="primary"
                                            className="w-full"
                                            onClick={() => setCheckoutOpen(true)}
                                            icon={<ShoppingCart className="w-5 h-5" />}
                                        >
                                            Thanh Toán
                                        </Button>
                                    </div>
                                </>
                            )}
                        </div>
                    </Card>
                </div>
            </div>

            {/* Checkout Modal */}
            <Modal
                isOpen={checkoutOpen}
                onClose={() => setCheckoutOpen(false)}
                title="Thanh Toán"
                footer={
                    <>
                        <Button variant="ghost" onClick={() => setCheckoutOpen(false)}>
                            Hủy
                        </Button>
                        <Button
                            variant="primary"
                            onClick={handleCheckout}
                            isLoading={createOrder.isPending}
                        >
                            Xác Nhận
                        </Button>
                    </>
                }
            >
                <div className="space-y-4">
                    <div className="flex justify-between text-lg">
                        <span>Tổng tiền:</span>
                        <span className="font-bold">{formatCurrency(total)}</span>
                    </div>

                    <Input
                        label="Khách đưa"
                        type="number"
                        value={customerPaid}
                        onChange={(e) => setCustomerPaid(e.target.value)}
                        placeholder="0"
                    />

                    <div className="grid grid-cols-3 gap-2">
                        {quickCash.map((amount) => (
                            <Button
                                key={amount}
                                variant="secondary"
                                size="sm"
                                onClick={() => setCustomerPaid(amount.toString())}
                            >
                                {amount >= 1000 ? `${amount / 1000}k` : amount}
                            </Button>
                        ))}
                    </div>

                    {customerPaid && (
                        <div className="flex justify-between text-lg font-semibold text-green-600">
                            <span>Tiền thừa:</span>
                            <span>{formatCurrency(parseInt(customerPaid) - total)}</span>
                        </div>
                    )}
                </div>
            </Modal>
        </div>
    );
};
