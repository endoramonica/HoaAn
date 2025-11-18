import React, { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/dialog';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Calendar } from '../ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '../ui/popover';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import { 
  Plus, 
  Calendar as CalendarIcon, 
  User, 
  Flag, 
  Save,
  AlertCircle
} from 'lucide-react';
import { mockUsers } from '../../services/mockData';
import { ROLES } from '../../utils/constants';
import { toast } from 'sonner';
import { format } from 'date-fns';

interface CreateTaskModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const priorities = [
  { value: 'low', label: 'Low', color: 'default' as const },
  { value: 'medium', label: 'Medium', color: 'secondary' as const },
  { value: 'high', label: 'High', color: 'destructive' as const },
];

export const CreateTaskModal: React.FC<CreateTaskModalProps> = ({
  open,
  onOpenChange,
}) => {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    assignedTo: '',
    priority: 'medium',
    dueDate: null as Date | null,
  });
  
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [calendarOpen, setCalendarOpen] = useState(false);

  // Filter staff members (exclude current user if they're manager)
  const staffMembers = mockUsers.filter(user => user.role === ROLES.STAFF && user.isActive);

  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'Task title is required';
    }
    if (!formData.description.trim()) {
      newErrors.description = 'Task description is required';
    }
    if (!formData.assignedTo) {
      newErrors.assignedTo = 'Please assign the task to a staff member';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) {
      toast.error('Please fix the errors in the form');
      return;
    }

    const taskData = {
      id: `task-${Date.now()}`,
      title: formData.title,
      description: formData.description,
      assignedTo: formData.assignedTo,
      assignedBy: 'current-manager-id', // Would be current user ID
      priority: formData.priority,
      status: 'pending',
      dueDate: formData.dueDate?.toISOString(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    console.log('Creating task:', taskData);
    toast.success('Task created successfully!');
    handleCancel();
  };

  const handleCancel = () => {
    setFormData({
      title: '',
      description: '',
      assignedTo: '',
      priority: 'medium',
      dueDate: null,
    });
    setErrors({});
    onOpenChange(false);
  };

  const getAssignedUser = (userId: string) => {
    return staffMembers.find(user => user.id === userId);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Plus className="h-5 w-5" />
            Create New Task
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Task Title */}
          <div>
            <Label htmlFor="title">Task Title *</Label>
            <Input
              id="title"
              placeholder="Enter task title"
              value={formData.title}
              onChange={(e) => handleInputChange('title', e.target.value)}
              className={errors.title ? 'border-red-500' : ''}
            />
            {errors.title && (
              <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.title}
              </p>
            )}
          </div>

          {/* Task Description */}
          <div>
            <Label htmlFor="description">Description *</Label>
            <Textarea
              id="description"
              placeholder="Describe the task in detail..."
              value={formData.description}
              onChange={(e) => handleInputChange('description', e.target.value)}
              rows={4}
              className={errors.description ? 'border-red-500' : ''}
            />
            {errors.description && (
              <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.description}
              </p>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4">
            {/* Assign To */}
            <div>
              <Label>Assign To *</Label>
              <Select 
                value={formData.assignedTo} 
                onValueChange={(value) => handleInputChange('assignedTo', value)}
              >
                <SelectTrigger className={errors.assignedTo ? 'border-red-500' : ''}>
                  <SelectValue placeholder="Select staff member" />
                </SelectTrigger>
                <SelectContent>
                  {staffMembers.map((staff) => (
                    <SelectItem key={staff.id} value={staff.id}>
                      <div className="flex items-center gap-2">
                        <User className="h-4 w-4" />
                        {staff.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {errors.assignedTo && (
                <p className="text-sm text-red-500 mt-1 flex items-center gap-1">
                  <AlertCircle className="h-3 w-3" />
                  {errors.assignedTo}
                </p>
              )}
            </div>

            {/* Priority */}
            <div>
              <Label>Priority</Label>
              <Select 
                value={formData.priority} 
                onValueChange={(value) => handleInputChange('priority', value)}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {priorities.map((priority) => (
                    <SelectItem key={priority.value} value={priority.value}>
                      <div className="flex items-center gap-2">
                        <Flag className="h-4 w-4" />
                        {priority.label}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Due Date */}
          <div>
            <Label>Due Date (Optional)</Label>
            <Popover open={calendarOpen} onOpenChange={setCalendarOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  className="w-full justify-start text-left font-normal"
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {formData.dueDate ? (
                    format(formData.dueDate, 'PPP')
                  ) : (
                    <span>Pick a date</span>
                  )}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0">
                <Calendar
                  mode="single"
                  selected={formData.dueDate || undefined}
                  onSelect={(date) => {
                    handleInputChange('dueDate', date);
                    setCalendarOpen(false);
                  }}
                  disabled={(date) => date < new Date()}
                  initialFocus
                />
              </PopoverContent>
            </Popover>
          </div>

          {/* Task Preview */}
          {formData.assignedTo && (
            <Card>
              <CardContent className="pt-6">
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Assigned to:</span>
                    <div className="flex items-center gap-2">
                      <User className="h-3 w-3" />
                      <span className="text-sm">{getAssignedUser(formData.assignedTo)?.name}</span>
                    </div>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Priority:</span>
                    <Badge variant={priorities.find(p => p.value === formData.priority)?.color}>
                      {priorities.find(p => p.value === formData.priority)?.label}
                    </Badge>
                  </div>
                  {formData.dueDate && (
                    <div className="flex items-center justify-between">
                      <span className="text-sm font-medium">Due date:</span>
                      <span className="text-sm">{format(formData.dueDate, 'PPP')}</span>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Action Buttons */}
          <div className="flex justify-end gap-3">
            <Button variant="outline" onClick={handleCancel}>
              Cancel
            </Button>
            <Button onClick={handleSubmit}>
              <Save className="h-4 w-4 mr-2" />
              Create Task
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};