import React, { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/dialog';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Separator } from '../ui/separator';
import { 
  Package, 
  Plus, 
  Minus, 
  RotateCcw, 
  Save,
  AlertCircle,
  TrendingUp,
  TrendingDown
} from 'lucide-react';
import { Product, InventoryItem } from '../../types';
import { toast } from 'sonner';

interface AdjustStockModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  product: Product | null;
  currentStock: number;
}

const adjustmentTypes = [
  { value: 'add', label: 'Add Stock', icon: Plus },
  { value: 'remove', label: 'Remove Stock', icon: Minus },
  { value: 'set', label: 'Set Stock Level', icon: RotateCcw },
];

const adjustmentReasons = [
  'Stock Delivery',
  'Product Return',
  'Damaged/Lost',
  'Stock Count Correction',
  'Expired Items',
  'Theft/Shrinkage', 
  'Transfer to Another Store',
  'Other'
];

export const AdjustStockModal: React.FC<AdjustStockModalProps> = ({
  open,
  onOpenChange,
  product,
  currentStock,
}) => {
  const [adjustmentType, setAdjustmentType] = useState('add');
  const [quantity, setQuantity] = useState('');
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  if (!product) return null;

  const calculateNewStock = () => {
    const qty = parseInt(quantity) || 0;
    switch (adjustmentType) {
      case 'add':
        return currentStock + qty;
      case 'remove':
        return Math.max(0, currentStock - qty);
      case 'set':
        return qty;
      default:
        return currentStock;
    }
  };

  const newStock = calculateNewStock();
  const stockDifference = newStock - currentStock;

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!quantity || parseInt(quantity) <= 0) {
      newErrors.quantity = 'Valid quantity is required';
    }

    if (adjustmentType === 'remove' && parseInt(quantity) > currentStock) {
      newErrors.quantity = 'Cannot remove more stock than available';
    }

    if (!reason) {
      newErrors.reason = 'Reason is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) {
      toast.error('Please fix the errors in the form');
      return;
    }

    const adjustmentData = {
      productId: product.id,
      productName: product.name,
      adjustmentType,
      quantity: parseInt(quantity),
      previousStock: currentStock,
      newStock,
      difference: stockDifference,
      reason,
      notes,
      timestamp: new Date().toISOString(),
    };

    console.log('Stock adjustment:', adjustmentData);
    toast.success(`Stock adjusted successfully! New stock level: ${newStock}`);
    handleCancel();
  };

  const handleCancel = () => {
    setAdjustmentType('add');
    setQuantity('');
    setReason('');
    setNotes('');
    setErrors({});
    onOpenChange(false);
  };

  const getStockStatus = (stock: number, minStock: number) => {
    if (stock === 0) return { status: 'Out of Stock', color: 'destructive' as const };
    if (stock <= minStock) return { status: 'Low Stock', color: 'secondary' as const };
    return { status: 'In Stock', color: 'default' as const };
  };

  const currentStatus = getStockStatus(currentStock, 10); // Assume min stock is 10
  const newStatus = getStockStatus(newStock, 10);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Adjust Stock - {product.name}
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Current Stock Info */}
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-muted-foreground">Current Stock</p>
                  <p className="text-2xl font-bold">{currentStock}</p>
                </div>
                <Badge variant={currentStatus.color}>
                  {currentStatus.status}
                </Badge>
              </div>
            </CardContent>
          </Card>

          {/* Adjustment Type */}
          <div>
            <Label>Adjustment Type</Label>
            <Select value={adjustmentType} onValueChange={setAdjustmentType}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {adjustmentTypes.map((type) => {
                  const IconComponent = type.icon;
                  return (
                    <SelectItem key={type.value} value={type.value}>
                      <div className="flex items-center gap-2">
                        <IconComponent className="h-4 w-4" />
                        {type.label}
                      </div>
                    </SelectItem>
                  );
                })}
              </SelectContent>
            </Select>
          </div>

          {/* Quantity */}
          <div>
            <Label htmlFor="quantity">
              {adjustmentType === 'set' ? 'New Stock Level' : 'Quantity'} *
            </Label>
            <Input
              id="quantity"
              type="number"
              placeholder="Enter quantity"
              value={quantity}
              onChange={(e) => {
                setQuantity(e.target.value);
                if (errors.quantity) {
                  setErrors(prev => ({ ...prev, quantity: '' }));
                }
              }}
              className={errors.quantity ? 'border-red-500' : ''}
            />
            {errors.quantity && (
              <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.quantity}
              </p>
            )}
          </div>

          {/* Stock Preview */}
          {quantity && (
            <Card>
              <CardContent className="pt-6">
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <span>Current Stock:</span>
                    <span className="font-medium">{currentStock}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span>Adjustment:</span>
                    <span className={`font-medium flex items-center gap-1 ${
                      stockDifference > 0 ? 'text-green-600' : stockDifference < 0 ? 'text-red-600' : ''
                    }`}>
                      {stockDifference > 0 ? (
                        <>
                          <TrendingUp className="h-3 w-3" />
                          +{stockDifference}
                        </>
                      ) : stockDifference < 0 ? (
                        <>
                          <TrendingDown className="h-3 w-3" />
                          {stockDifference}
                        </>
                      ) : (
                        '0'
                      )}
                    </span>
                  </div>
                  <Separator />
                  <div className="flex items-center justify-between">
                    <span>New Stock:</span>
                    <div className="flex items-center gap-2">
                      <span className="font-bold text-lg">{newStock}</span>
                      <Badge variant={newStatus.color}>
                        {newStatus.status}
                      </Badge>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Reason */}
          <div>
            <Label>Reason *</Label>
            <Select value={reason} onValueChange={(value) => {
              setReason(value);
              if (errors.reason) {
                setErrors(prev => ({ ...prev, reason: '' }));
              }
            }}>
              <SelectTrigger className={errors.reason ? 'border-red-500' : ''}>
                <SelectValue placeholder="Select reason" />
              </SelectTrigger>
              <SelectContent>
                {adjustmentReasons.map((reasonOption) => (
                  <SelectItem key={reasonOption} value={reasonOption}>
                    {reasonOption}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.reason && (
              <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.reason}
              </p>
            )}
          </div>

          {/* Notes */}
          <div>
            <Label htmlFor="notes">Additional Notes</Label>
            <Textarea
              id="notes"
              placeholder="Add any additional details..."
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={3}
            />
          </div>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={handleCancel}>
              Cancel
            </Button>
            <Button onClick={handleSubmit}>
              <Save className="h-4 w-4 mr-2" />
              Adjust Stock
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};