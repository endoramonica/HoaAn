import React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../ui/dialog';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Separator } from '../ui/separator';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { 
  Clock, 
  User, 
  Calendar,
  Flag,
  CheckCircle,
  PlayCircle,
  AlertCircle,
  Edit,
  MessageSquare
} from 'lucide-react';
import { mockUsers } from '../../services/mockData';
import { Task } from '../../types';
import { toast } from 'sonner';

interface TaskDetailsModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  task: Task | null;
  onTaskUpdate?: (taskId: string, updates: Partial<Task>) => void;
}

export const TaskDetailsModal: React.FC<TaskDetailsModalProps> = ({
  open,
  onOpenChange,
  task,
  onTaskUpdate,
}) => {
  if (!task) return null;

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed':
        return <CheckCircle className="w-4 h-4 text-green-600" />;
      case 'in_progress':
        return <Clock className="w-4 h-4 text-blue-600" />;
      default:
        return <AlertCircle className="w-4 h-4 text-yellow-600" />;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'default';
      case 'in_progress':
        return 'secondary';
      default:
        return 'outline';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'high':
        return 'destructive';
      case 'medium':
        return 'secondary';
      default:
        return 'outline';
    }
  };

  const getAssignedUser = (userId: string) => {
    return mockUsers.find(u => u.id === userId);
  };

  const getCreatedByUser = (userId: string) => {
    return mockUsers.find(u => u.id === userId);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return 'No due date';
    return new Date(dateString).toLocaleString();
  };

  const handleStartTask = () => {
    if (onTaskUpdate) {
      onTaskUpdate(task.id, { status: 'in_progress', updatedAt: new Date().toISOString() });
    }
    toast.success('Task started!');
    onOpenChange(false);
  };

  const handleCompleteTask = () => {
    if (onTaskUpdate) {
      onTaskUpdate(task.id, { status: 'completed', updatedAt: new Date().toISOString() });
    }
    toast.success('Task completed!');
    onOpenChange(false);
  };

  const handleEditTask = () => {
    toast.info('Edit task functionality - coming soon!');
  };

  const assignedUser = getAssignedUser(task.assignedTo);
  const createdByUser = getCreatedByUser(task.assignedBy);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <div className="flex items-center justify-between">
            <DialogTitle className="flex items-center gap-2">
              {getStatusIcon(task.status)}
              Task Details
            </DialogTitle>
            <div className="flex gap-2">
              {task.status === 'pending' && (
                <Button size="sm" onClick={handleStartTask}>
                  <PlayCircle className="h-4 w-4 mr-2" />
                  Start Task
                </Button>
              )}
              {task.status === 'in_progress' && (
                <Button size="sm" onClick={handleCompleteTask}>
                  <CheckCircle className="h-4 w-4 mr-2" />
                  Complete
                </Button>
              )}
              <Button variant="outline" size="sm" onClick={handleEditTask}>
                <Edit className="h-4 w-4 mr-2" />
                Edit
              </Button>
            </div>
          </div>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Task Header */}
          <Card>
            <CardContent className="pt-6">
              <div className="space-y-4">
                <div>
                  <h3 className="text-xl font-semibold mb-2">{task.title}</h3>
                  <div className="flex items-center gap-2 mb-3">
                    <Badge variant={getStatusColor(task.status)}>
                      {task.status}
                    </Badge>
                    <Badge variant={getPriorityColor(task.priority)}>
                      {task.priority} priority
                    </Badge>
                  </div>
                </div>
                
                <p className="text-muted-foreground">
                  {task.description}
                </p>
              </div>
            </CardContent>
          </Card>

          {/* Task Information */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Assigned To */}
            <Card>
              <CardHeader>
                <CardTitle className="text-sm flex items-center gap-2">
                  <User className="h-4 w-4" />
                  Assigned To
                </CardTitle>
              </CardHeader>
              <CardContent>
                {assignedUser ? (
                  <div className="flex items-center gap-3">
                    <Avatar>
                      <AvatarImage src={assignedUser.avatar} />
                      <AvatarFallback>
                        {assignedUser.name.split(' ').map(n => n[0]).join('').toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <p className="font-medium">{assignedUser.name}</p>
                      <p className="text-sm text-muted-foreground">{assignedUser.email}</p>
                    </div>
                  </div>
                ) : (
                  <p className="text-muted-foreground">Unknown user</p>
                )}
              </CardContent>
            </Card>

            {/* Created By */}
            <Card>
              <CardHeader>
                <CardTitle className="text-sm flex items-center gap-2">
                  <User className="h-4 w-4" />
                  Created By
                </CardTitle>
              </CardHeader>
              <CardContent>
                {createdByUser ? (
                  <div className="flex items-center gap-3">
                    <Avatar>
                      <AvatarImage src={createdByUser.avatar} />
                      <AvatarFallback>
                        {createdByUser.name.split(' ').map(n => n[0]).join('').toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <p className="font-medium">{createdByUser.name}</p>
                      <p className="text-sm text-muted-foreground">{createdByUser.email}</p>
                    </div>
                  </div>
                ) : (
                  <p className="text-muted-foreground">Unknown user</p>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Dates and Timeline */}
          <Card>
            <CardHeader>
              <CardTitle className="text-sm flex items-center gap-2">
                <Calendar className="h-4 w-4" />
                Timeline
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-sm text-muted-foreground">Created:</span>
                <span className="text-sm">{formatDate(task.createdAt)}</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-muted-foreground">Last Updated:</span>
                <span className="text-sm">{formatDate(task.updatedAt)}</span>
              </div>
              {task.dueDate && (
                <div className="flex justify-between items-center">
                  <span className="text-sm text-muted-foreground">Due Date:</span>
                  <span className={`text-sm ${
                    new Date(task.dueDate) < new Date() && task.status !== 'completed'
                      ? 'text-red-600 font-medium'
                      : ''
                  }`}>
                    {formatDate(task.dueDate)}
                    {new Date(task.dueDate) < new Date() && task.status !== 'completed' && (
                      <span className="ml-1 text-xs">(Overdue)</span>
                    )}
                  </span>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Progress Indicators */}
          <Card>
            <CardHeader>
              <CardTitle className="text-sm">Progress</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${
                      task.status === 'pending' ? 'bg-yellow-500' :
                      task.status === 'in_progress' ? 'bg-blue-500' :
                      'bg-green-500'
                    }`} />
                    <span className="text-sm">
                      {task.status === 'pending' ? 'Waiting to Start' :
                       task.status === 'in_progress' ? 'In Progress' :
                       'Completed'}
                    </span>
                  </div>
                  <span className="text-xs text-muted-foreground">
                    {task.status === 'completed' ? '100%' :
                     task.status === 'in_progress' ? '50%' : '0%'}
                  </span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div 
                    className={`h-2 rounded-full ${
                      task.status === 'completed' ? 'bg-green-500' :
                      task.status === 'in_progress' ? 'bg-blue-500' :
                      'bg-yellow-500'
                    }`}
                    style={{ 
                      width: task.status === 'completed' ? '100%' :
                             task.status === 'in_progress' ? '50%' : '10%'
                    }}
                  />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Notes Section */}
          <Card>
            <CardHeader>
              <CardTitle className="text-sm flex items-center gap-2">
                <MessageSquare className="h-4 w-4" />
                Notes & Comments
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-center py-8 text-muted-foreground">
                <MessageSquare className="h-8 w-8 mx-auto mb-2 opacity-50" />
                <p className="text-sm">No notes or comments yet</p>
                <Button variant="outline" size="sm" className="mt-2">
                  Add Note
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </DialogContent>
    </Dialog>
  );
};