import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Button } from './ui/button';
import { ArrowLeft, Home } from 'lucide-react';
import { useI18n } from '../contexts/I18nContext';

interface BackToDashboardProps {
  className?: string;
  variant?: 'button' | 'link';
  showText?: boolean;
}

export const BackToDashboard: React.FC<BackToDashboardProps> = ({
  className = '',
  variant = 'button',
  showText = true,
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useI18n();

  const handleGoBack = () => {
    // If we can go back in history and we're not on the dashboard, go back
    if (window.history.length > 1 && location.pathname !== '/dashboard') {
      navigate(-1);
    } else {
      // Otherwise go to dashboard
      navigate('/dashboard');
    }
  };

  const handleGoToDashboard = () => {
    navigate('/dashboard');
  };

  if (variant === 'link') {
    return (
      <div className={`flex items-center gap-2 ${className}`}>
        <Button
          variant="ghost"
          size="sm"
          onClick={handleGoBack}
          className="text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="w-4 h-4" />
          {showText && <span className="ml-1">Back</span>}
        </Button>
        <span className="text-muted-foreground">|</span>
        <Button
          variant="ghost"
          size="sm"
          onClick={handleGoToDashboard}
          className="text-muted-foreground hover:text-foreground"
        >
          <Home className="w-4 h-4" />
          {showText && <span className="ml-1">Dashboard</span>}
        </Button>
      </div>
    );
  }

  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <Button variant="outline" size="sm" onClick={handleGoBack}>
        <ArrowLeft className="w-4 h-4 mr-2" />
        {showText ? 'Go Back' : ''}
      </Button>
      <Button variant="outline" size="sm" onClick={handleGoToDashboard}>
        <Home className="w-4 h-4 mr-2" />
        {showText ? 'Dashboard' : ''}
      </Button>
    </div>
  );
};