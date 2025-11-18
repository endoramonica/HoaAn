import React, { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/dialog';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import { 
  Package, 
  DollarSign, 
  Barcode, 
  Image as ImageIcon,
  Save,
  AlertCircle
} from 'lucide-react';
import { toast } from 'sonner';

interface AddProductModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const categories = [
  'Electronics',
  'Clothing',
  'Accessories', 
  'Books',
  'Home & Garden',
  'Sports',
  'Food & Beverages',
  'Other'
];

export const AddProductModal: React.FC<AddProductModalProps> = ({
  open,
  onOpenChange,
}) => {
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    barcode: '',
    price: '',
    cost: '',
    category: '',
    initialStock: '',
    minStock: '',
    maxStock: '',
    image: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Product name is required';
    }
    if (!formData.barcode.trim()) {
      newErrors.barcode = 'Barcode is required';
    }
    if (!formData.price || parseFloat(formData.price) <= 0) {
      newErrors.price = 'Valid price is required';
    }
    if (!formData.cost || parseFloat(formData.cost) <= 0) {
      newErrors.cost = 'Valid cost is required';
    }
    if (!formData.category) {
      newErrors.category = 'Category is required';
    }
    if (!formData.initialStock || parseInt(formData.initialStock) < 0) {
      newErrors.initialStock = 'Valid initial stock is required';
    }
    if (!formData.minStock || parseInt(formData.minStock) < 0) {
      newErrors.minStock = 'Valid minimum stock is required';
    }
    if (!formData.maxStock || parseInt(formData.maxStock) <= 0) {
      newErrors.maxStock = 'Valid maximum stock is required';
    }

    // Validate cost vs price
    if (formData.cost && formData.price && parseFloat(formData.cost) > parseFloat(formData.price)) {
      newErrors.cost = 'Cost cannot be higher than price';
    }

    // Validate stock levels
    if (formData.minStock && formData.maxStock && parseInt(formData.minStock) > parseInt(formData.maxStock)) {
      newErrors.minStock = 'Minimum stock cannot be higher than maximum stock';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) {
      toast.error('Please fix the errors in the form');
      return;
    }

    const productData = {
      id: `product-${Date.now()}`,
      name: formData.name,
      description: formData.description,
      barcode: formData.barcode,
      price: parseFloat(formData.price),
      cost: parseFloat(formData.cost),
      category: formData.category,
      image: formData.image || undefined,
      isActive: true,
      inventory: {
        quantity: parseInt(formData.initialStock),
        minStock: parseInt(formData.minStock),
        maxStock: parseInt(formData.maxStock),
      }
    };

    console.log('Creating product:', productData);
    toast.success('Product added successfully!');
    handleCancel();
  };

  const handleCancel = () => {
    setFormData({
      name: '',
      description: '',
      barcode: '',
      price: '',
      cost: '',
      category: '',
      initialStock: '',
      minStock: '',
      maxStock: '',
      image: '',
    });
    setErrors({});
    onOpenChange(false);
  };

  const generateBarcode = () => {
    const barcode = Math.random().toString().slice(2, 15);
    setFormData(prev => ({ ...prev, barcode }));
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Add New Product
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Basic Information */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="productName">Product Name *</Label>
                  <Input
                    id="productName"
                    placeholder="Enter product name"
                    value={formData.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                    className={errors.name ? 'border-red-500' : ''}
                  />
                  {errors.name && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.name}
                    </p>
                  )}
                </div>
                
                <div>
                  <Label htmlFor="category">Category *</Label>
                  <Select value={formData.category} onValueChange={(value) => handleInputChange('category', value)}>
                    <SelectTrigger className={errors.category ? 'border-red-500' : ''}>
                      <SelectValue placeholder="Select category" />
                    </SelectTrigger>
                    <SelectContent>
                      {categories.map((category) => (
                        <SelectItem key={category} value={category}>
                          {category}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {errors.category && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.category}
                    </p>
                  )}
                </div>
              </div>

              <div>
                <Label htmlFor="description">Description</Label>
                <Textarea
                  id="description"
                  placeholder="Enter product description"
                  value={formData.description}
                  onChange={(e) => handleInputChange('description', e.target.value)}
                  rows={3}
                />
              </div>

              <div>
                <Label htmlFor="barcode">Barcode *</Label>
                <div className="flex gap-2">
                  <Input
                    id="barcode"
                    placeholder="Enter or generate barcode"
                    value={formData.barcode}
                    onChange={(e) => handleInputChange('barcode', e.target.value)}
                    className={errors.barcode ? 'border-red-500' : ''}
                  />
                  <Button type="button" variant="outline" onClick={generateBarcode}>
                    <Barcode className="h-4 w-4 mr-2" />
                    Generate
                  </Button>
                </div>
                {errors.barcode && (
                  <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.barcode}
                  </p>
                )}
              </div>

              <div>
                <Label htmlFor="image">Image URL</Label>
                <Input
                  id="image"
                  placeholder="Enter image URL (optional)"
                  value={formData.image}
                  onChange={(e) => handleInputChange('image', e.target.value)}
                />
              </div>
            </CardContent>
          </Card>

          {/* Pricing */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="cost">Cost Price *</Label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      id="cost"
                      type="number"
                      step="0.01"
                      placeholder="0.00"
                      value={formData.cost}
                      onChange={(e) => handleInputChange('cost', e.target.value)}
                      className={`pl-10 ${errors.cost ? 'border-red-500' : ''}`}
                    />
                  </div>
                  {errors.cost && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.cost}
                    </p>
                  )}
                </div>
                
                <div>
                  <Label htmlFor="price">Selling Price *</Label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      id="price"
                      type="number"
                      step="0.01"
                      placeholder="0.00"
                      value={formData.price}
                      onChange={(e) => handleInputChange('price', e.target.value)}
                      className={`pl-10 ${errors.price ? 'border-red-500' : ''}`}
                    />
                  </div>
                  {errors.price && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.price}
                    </p>
                  )}
                </div>
              </div>

              {/* Profit Calculation */}
              {formData.cost && formData.price && parseFloat(formData.cost) <= parseFloat(formData.price) && (
                <div className="p-3 bg-green-50 border border-green-200 rounded-lg">
                  <div className="flex items-center justify-between text-sm">
                    <span>Profit Margin:</span>
                    <Badge variant="default">
                      ${(parseFloat(formData.price) - parseFloat(formData.cost)).toFixed(2)} 
                      ({(((parseFloat(formData.price) - parseFloat(formData.cost)) / parseFloat(formData.price)) * 100).toFixed(1)}%)
                    </Badge>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Inventory Settings */}
          <Card>
            <CardContent className="pt-6 space-y-4">
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <Label htmlFor="initialStock">Initial Stock *</Label>
                  <Input
                    id="initialStock"
                    type="number"
                    placeholder="0"
                    value={formData.initialStock}
                    onChange={(e) => handleInputChange('initialStock', e.target.value)}
                    className={errors.initialStock ? 'border-red-500' : ''}
                  />
                  {errors.initialStock && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.initialStock}
                    </p>
                  )}
                </div>
                
                <div>
                  <Label htmlFor="minStock">Min Stock *</Label>
                  <Input
                    id="minStock"
                    type="number"
                    placeholder="0"
                    value={formData.minStock}
                    onChange={(e) => handleInputChange('minStock', e.target.value)}
                    className={errors.minStock ? 'border-red-500' : ''}
                  />
                  {errors.minStock && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.minStock}
                    </p>
                  )}
                </div>
                
                <div>
                  <Label htmlFor="maxStock">Max Stock *</Label>
                  <Input
                    id="maxStock"
                    type="number"
                    placeholder="0"
                    value={formData.maxStock}
                    onChange={(e) => handleInputChange('maxStock', e.target.value)}
                    className={errors.maxStock ? 'border-red-500' : ''}
                  />
                  {errors.maxStock && (
                    <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.maxStock}
                    </p>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={handleCancel}>
              Cancel
            </Button>
            <Button onClick={handleSubmit}>
              <Save className="h-4 w-4 mr-2" />
              Add Product
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};