import React, { useState, useMemo } from 'react';
import { useI18n } from '../contexts/I18nContext';
import { PermissionWrapper } from '../components/PermissionWrapper';
import { AddProductModal } from '../components/modals/AddProductModal';
import { AdjustStockModal } from '../components/modals/AdjustStockModal';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { usePagination } from '../hooks/usePagination';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Progress } from '../components/ui/progress';
import { 
  Search, 
  Plus, 
  Package, 
  AlertTriangle, 
  Edit, 
  BarChart3,
  Download,
  Upload
} from 'lucide-react';
import { mockProducts, mockInventory } from '../services/mockData';
import { PERMISSIONS } from '../utils/constants';
import { Product, InventoryItem } from '../types';
import { toast } from 'sonner';

interface ProductWithInventory extends Product {
  inventory: InventoryItem;
}

export const InventoryPage: React.FC = () => {
  const { t } = useI18n();
  const [searchTerm, setSearchTerm] = useState('');
  const [addProductModalOpen, setAddProductModalOpen] = useState(false);
  const [adjustStockModalOpen, setAdjustStockModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [selectedProductStock, setSelectedProductStock] = useState(0);

  // Combine products with their inventory data
  const productsWithInventory: ProductWithInventory[] = mockProducts.map(product => {
    const inventory = mockInventory.find(inv => inv.productId === product.id);
    return {
      ...product,
      inventory: inventory || {
        id: '',
        productId: product.id,
        storeId: 'store-1',
        quantity: 0,
        minStock: 10,
        maxStock: 100,
        lastUpdated: new Date().toISOString(),
      }
    };
  });

  const filteredProducts = useMemo(() => {
    return productsWithInventory.filter(product =>
      product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.category.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.barcode.includes(searchTerm)
    );
  }, [productsWithInventory, searchTerm]);

  const {
    currentPage,
    totalPages,
    itemsPerPage,
    totalItems,
    paginatedData: paginatedProducts,
    goToPage,
    setItemsPerPage,
  } = usePagination({
    data: filteredProducts,
    initialItemsPerPage: 15,
  });

  const lowStockProducts = filteredProducts.filter(p => p.inventory.quantity <= p.inventory.minStock);
  const outOfStockProducts = filteredProducts.filter(p => p.inventory.quantity === 0);

  const getStockStatus = (quantity: number, minStock: number) => {
    if (quantity === 0) return { status: 'Out of Stock', color: 'destructive' as const };
    if (quantity <= minStock) return { status: 'Low Stock', color: 'secondary' as const };
    return { status: 'In Stock', color: 'default' as const };
  };

  const getStockProgress = (quantity: number, maxStock: number) => {
    return Math.min((quantity / maxStock) * 100, 100);
  };

  const handleAddProduct = () => {
    setAddProductModalOpen(true);
  };

  const handleEditProduct = (product: ProductWithInventory) => {
    console.log('Editing product:', product.id);
    toast.info(`Edit functionality for ${product.name} - coming soon!`);
  };

  const handleAdjustStock = (product: ProductWithInventory) => {
    setSelectedProduct(product);
    setSelectedProductStock(product.inventory.quantity);
    setAdjustStockModalOpen(true);
  };

  const handleExportInventory = () => {
    console.log('Exporting inventory:', filteredProducts);
    const csvData = filteredProducts.map(product => ({
      Name: product.name,
      Category: product.category,
      Barcode: product.barcode,
      Price: product.price,
      Cost: product.cost,
      'Current Stock': product.inventory.quantity,
      'Min Stock': product.inventory.minStock,
      'Max Stock': product.inventory.maxStock,
      Status: getStockStatus(product.inventory.quantity, product.inventory.minStock).status,
    }));
    
    toast.success(`Exporting ${filteredProducts.length} products to CSV`);
    console.log('CSV Data:', csvData);
  };

  const handleImportProducts = () => {
    toast.info('Import products functionality - coming soon!');
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-semibold">{t('nav.inventory')}</h1>
          <p className="text-muted-foreground">
            Manage your store inventory and stock levels
          </p>
        </div>
        
        <div className="flex gap-2">
          <PermissionWrapper permission={PERMISSIONS.MANAGE_INVENTORY}>
            <Button variant="outline" onClick={handleImportProducts}>
              <Upload className="w-4 h-4 mr-2" />
              Import
            </Button>
            <Button onClick={handleAddProduct}>
              <Plus className="w-4 h-4 mr-2" />
              Add Product
            </Button>
          </PermissionWrapper>
        </div>
      </div>

      {/* Quick Stats */}
      <div className="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Products</CardTitle>
            <Package className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{totalItems}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Low Stock</CardTitle>
            <AlertTriangle className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">{lowStockProducts.length}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Out of Stock</CardTitle>
            <AlertTriangle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">{outOfStockProducts.length}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Value</CardTitle>
            <BarChart3 className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              ${filteredProducts.reduce((sum, p) => sum + (p.inventory.quantity * p.cost), 0).toFixed(0)}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Search & Filter</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
                <Input
                  placeholder="Search products by name, category, or barcode..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <Button variant="outline" onClick={handleExportInventory}>
              <Download className="w-4 h-4 mr-2" />
              Export
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle>Products ({totalItems})</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {totalItems === 0 ? (
            <div className="text-center py-8">
              <Package className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
              <p className="text-muted-foreground">No products found</p>
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Product</TableHead>
                    <TableHead>Category</TableHead>
                    <TableHead>Barcode</TableHead>
                    <TableHead>Price</TableHead>
                    <TableHead>Stock</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Stock Level</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {paginatedProducts.map((product) => {
                    const stockStatus = getStockStatus(
                      product.inventory.quantity,
                      product.inventory.minStock
                    );
                    const stockProgress = getStockProgress(
                      product.inventory.quantity,
                      product.inventory.maxStock
                    );

                    return (
                      <TableRow key={product.id}>
                        <TableCell>
                          <div className="flex items-center space-x-3">
                            <div className="w-10 h-10 bg-gray-200 rounded overflow-hidden">
                              {product.image && (
                                <img
                                  src={product.image}
                                  alt={product.name}
                                  className="w-full h-full object-cover"
                                />
                              )}
                            </div>
                            <div>
                              <p className="font-medium">{product.name}</p>
                              <p className="text-sm text-muted-foreground">
                                {product.description}
                              </p>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>{product.category}</TableCell>
                        <TableCell className="font-mono text-sm">
                          {product.barcode}
                        </TableCell>
                        <TableCell className="font-medium">
                          ${product.price.toFixed(2)}
                        </TableCell>
                        <TableCell>
                          <div>
                            <p className="font-medium">{product.inventory.quantity}</p>
                            <p className="text-xs text-muted-foreground">
                              Min: {product.inventory.minStock} | Max: {product.inventory.maxStock}
                            </p>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant={stockStatus.color}>
                            {stockStatus.status}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="w-20">
                            <Progress 
                              value={stockProgress} 
                              className="h-2"
                            />
                            <p className="text-xs text-muted-foreground mt-1">
                              {stockProgress.toFixed(0)}%
                            </p>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="flex space-x-1">
                            <Button size="sm" variant="outline" onClick={() => handleEditProduct(product)}>
                              <Edit className="w-3 h-3" />
                            </Button>
                            <PermissionWrapper permission={PERMISSIONS.ADJUST_STOCK}>
                              <Button size="sm" variant="outline" onClick={() => handleAdjustStock(product)}>
                                Adjust
                              </Button>
                            </PermissionWrapper>
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>

              {/* Pagination */}
              <PaginationCustom
                currentPage={currentPage}
                totalPages={totalPages}
                itemsPerPage={itemsPerPage}
                totalItems={totalItems}
                onPageChange={goToPage}
                onItemsPerPageChange={setItemsPerPage}
              />
            </>
          )}
        </CardContent>
      </Card>
      
      {/* Modals */}
      <AddProductModal
        open={addProductModalOpen}
        onOpenChange={setAddProductModalOpen}
      />
      <AdjustStockModal
        open={adjustStockModalOpen}
        onOpenChange={setAdjustStockModalOpen}
        product={selectedProduct}
        currentStock={selectedProductStock}
      />
    </div>
  );
};