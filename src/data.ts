import { colors } from './theme';
import type { Category, Place } from './types';

export const categories: Category[] = [
  { id: 'servicos', label: 'Serviços', icon: 'briefcase-outline', color: '#DDEBE8' },
  { id: 'saude', label: 'Saúde', icon: 'medkit-outline', color: '#FCE2DE' },
  { id: 'lazer', label: 'Lazer', icon: 'happy-outline', color: '#FFF0C9' },
  { id: 'esporte', label: 'Esporte', icon: 'football-outline', color: '#E6E1FA' },
  { id: 'turismo', label: 'Turismo', icon: 'camera-outline', color: '#DCEBF8' },
];

export const places: Place[] = [
  {
    id: '1',
    name: 'Parque da Cidade',
    category: 'Lazer ao ar livre',
    distance: '1,2 km',
    neighborhood: 'Centro',
    rating: 4.9,
    reviews: 128,
    accessibilityScore: 96,
    description: 'Trilhas planas, banheiros adaptados e equipe treinada para receber todos.',
    color: '#BFD8CB',
    icon: 'leaf-outline',
    verified: true,
    features: [
      { label: 'Rampa', icon: 'trending-up-outline' },
      { label: 'Banheiro PCD', icon: 'accessibility-outline' },
      { label: 'Piso tátil', icon: 'trail-sign-outline' },
    ],
  },
  {
    id: '2',
    name: 'Café Girassol',
    category: 'Gastronomia',
    distance: '850 m',
    neighborhood: 'Jardins',
    rating: 4.8,
    reviews: 86,
    accessibilityScore: 92,
    description: 'Entrada nivelada, mesas com boa circulação e cardápio em braile.',
    color: '#F2D49B',
    icon: 'cafe-outline',
    features: [
      { label: 'Sem degraus', icon: 'remove-outline' },
      { label: 'Braile', icon: 'ellipsis-horizontal-outline' },
      { label: 'Cão-guia', icon: 'paw-outline' },
    ],
  },
  {
    id: '3',
    name: 'Centro Esportivo Viver',
    category: 'Esporte adaptado',
    distance: '2,4 km',
    neighborhood: 'Vila Nova',
    rating: 4.7,
    reviews: 64,
    accessibilityScore: 89,
    description: 'Modalidades inclusivas, vestiários adaptados e estacionamento reservado.',
    color: '#C8DDF0',
    icon: 'basketball-outline',
    verified: true,
    features: [
      { label: 'Esporte PCD', icon: 'medal-outline' },
      { label: 'Vestiário', icon: 'shirt-outline' },
      { label: 'Vaga PCD', icon: 'car-outline' },
    ],
  },
];

export const leaderboard = [
  { name: 'Marina Alves', points: 2840, reviews: 42, initials: 'MA', color: colors.coral },
  { name: 'João Pedro', points: 2610, reviews: 38, initials: 'JP', color: colors.blue },
  { name: 'Bia Santos', points: 2390, reviews: 35, initials: 'BS', color: colors.amber },
  { name: 'Rafael Lima', points: 1960, reviews: 29, initials: 'RL', color: colors.green },
];
