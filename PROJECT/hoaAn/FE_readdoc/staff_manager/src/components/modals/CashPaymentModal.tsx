import React, { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/dialog';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Separator } from '../ui/separator';
import { 
  DollarSign, 
  Calculator, 
  CheckCircle, 
  AlertCircle,
  Banknote
} from 'lucide-react';
import { toast } from 'sonner';

interface CashPaymentModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  orderTotal: number;
  onPaymentComplete: () => void;
}

const quickAmounts = [5, 10, 20, 50, 100];

export const CashPaymentModal: React.FC<CashPaymentModalProps> = ({
  open,
  onOpenChange,
  orderTotal,
  onPaymentComplete,
}) => {
  const [cashReceived, setCashReceived] = useState('');
  const [error, setError] = useState('');

  const cashAmount = parseFloat(cashReceived) || 0;
  const change = cashAmount - orderTotal;
  const isValidPayment = cashAmount >= orderTotal;

  useEffect(() => {
    if (open) {
      setCashReceived('');
      setError('');
    }
  }, [open]);

  const handleAmountChange = (value: string) => {
    setCashReceived(value);
    setError('');
  };

  const handleQuickAmount = (amount: number) => {
    setCashReceived(amount.toString());
    setError('');
  };

  const handleExactAmount = () => {
    setCashReceived(orderTotal.toString());
    setError('');
  };

  const handlePayment = () => {
    if (!cashReceived) {
      setError('Please enter the cash amount received');
      return;
    }

    if (cashAmount < orderTotal) {
      setError('Cash received is less than the order total');
      return;
    }

    // Process payment
    const paymentData = {
      method: 'cash',
      orderTotal,
      cashReceived: cashAmount,
      change,
      timestamp: new Date().toISOString(),
    };

    console.log('Processing cash payment:', paymentData);
    toast.success('Cash payment processed successfully!');
    
    onPaymentComplete();
    onOpenChange(false);
  };

  const handleCancel = () => {
    setCashReceived('');
    setError('');
    onOpenChange(false);
  };

  const formatCurrency = (amount: number) => `$${amount.toFixed(2)}`;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <DollarSign className="h-5 w-5" />
            Cash Payment
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Order Total */}
          <Card>
            <CardContent className="pt-6">
              <div className="text-center">
                <p className="text-sm text-muted-foreground mb-1">Order Total</p>
                <p className="text-3xl font-bold text-primary">
                  {formatCurrency(orderTotal)}
                </p>
              </div>
            </CardContent>
          </Card>

          {/* Quick Amount Buttons */}
          <div>
            <Label className="text-sm font-medium mb-3 block">Quick Amounts</Label>
            <div className="grid grid-cols-3 gap-2 mb-2">
              {quickAmounts.map((amount) => (
                <Button
                  key={amount}
                  variant="outline"
                  size="sm"
                  onClick={() => handleQuickAmount(amount)}
                  className="h-12"
                >
                  <Banknote className="h-4 w-4 mr-1" />
                  ${amount}
                </Button>
              ))}
              <Button
                variant="outline"
                size="sm"
                onClick={handleExactAmount}
                className="h-12 col-span-3"
              >
                <CheckCircle className="h-4 w-4 mr-2" />
                Exact Amount ({formatCurrency(orderTotal)})
              </Button>
            </div>
          </div>

          {/* Cash Received Input */}
          <div>
            <Label htmlFor="cashReceived">Cash Received</Label>
            <div className="relative">
              <DollarSign className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                id="cashReceived"
                type="number"
                step="0.01"
                placeholder="0.00"
                value={cashReceived}
                onChange={(e) => handleAmountChange(e.target.value)}
                className={`pl-10 text-lg h-12 ${error ? 'border-red-500' : ''}`}
                autoFocus
              />
            </div>
            {error && (
              <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {error}
              </p>
            )}
          </div>

          {/* Change Calculation */}
          {cashReceived && (
            <Card className={isValidPayment ? 'border-green-200 bg-green-50' : 'border-red-200 bg-red-50'}>
              <CardContent className="pt-6">
                <div className="space-y-2">
                  <div className="flex justify-between items-center">
                    <span className="text-sm">Cash Received:</span>
                    <span className="font-medium">{formatCurrency(cashAmount)}</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-sm">Order Total:</span>
                    <span className="font-medium">{formatCurrency(orderTotal)}</span>
                  </div>
                  <Separator />
                  <div className="flex justify-between items-center">
                    <span className="font-medium">Change:</span>
                    <div className="text-right">
                      <span className={`text-lg font-bold ${
                        change >= 0 ? 'text-green-600' : 'text-red-600'
                      }`}>
                        {formatCurrency(Math.abs(change))}
                      </span>
                      {change < 0 && (
                        <p className="text-xs text-red-600">Insufficient payment</p>
                      )}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Payment Instructions */}
          {isValidPayment && change > 0 && (
            <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg">
              <div className="flex items-start gap-2">
                <Calculator className="h-4 w-4 text-blue-600 mt-0.5" />
                <div>
                  <p className="text-sm font-medium text-blue-800">
                    Return Change: {formatCurrency(change)}
                  </p>
                  <p className="text-xs text-blue-600">
                    Please count and return the exact change to the customer
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Action Buttons */}
          <div className="flex gap-3">
            <Button variant="outline" onClick={handleCancel} className="flex-1">
              Cancel
            </Button>
            <Button 
              onClick={handlePayment} 
              disabled={!isValidPayment}
              className="flex-1"
            >
              <CheckCircle className="h-4 w-4 mr-2" />
              Complete Payment
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};