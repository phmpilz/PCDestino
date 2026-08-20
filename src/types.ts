import type { ComponentProps } from 'react';
import type { Ionicons } from '@expo/vector-icons';

export type IconName = ComponentProps<typeof Ionicons>['name'];

export type Category = {
  id: string;
  label: string;
  icon: IconName;
  color: string;
};

export type AccessibilityFeature = {
  label: string;
  icon: IconName;
};

export type Place = {
  id: string;
  name: string;
  category: string;
  distance: string;
  neighborhood: string;
  rating: number;
  reviews: number;
  accessibilityScore: number;
  description: string;
  color: string;
  icon: IconName;
  features: AccessibilityFeature[];
  verified?: boolean;
};

export type TabId = 'inicio' | 'explorar' | 'contribuir' | 'ranking' | 'perfil';
