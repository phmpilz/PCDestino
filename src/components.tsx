import React from 'react';
import {
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
  type StyleProp,
  type ViewStyle,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { colors, radii } from './theme';
import type { Category, IconName, Place } from './types';

export function IconButton({
  icon,
  label,
  onPress,
  active = false,
}: {
  icon: IconName;
  label: string;
  onPress?: () => void;
  active?: boolean;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      style={({ pressed }) => [styles.iconButton, active && styles.iconButtonActive, pressed && styles.pressed]}
    >
      <Ionicons name={icon} size={21} color={active ? colors.surface : colors.ink} />
    </Pressable>
  );
}

export function SearchField({
  value,
  onChangeText,
  placeholder = 'Busque um local ou serviço',
  style,
}: {
  value: string;
  onChangeText: (value: string) => void;
  placeholder?: string;
  style?: StyleProp<ViewStyle>;
}) {
  return (
    <View style={[styles.search, style]}>
      <Ionicons name="search-outline" size={22} color={colors.muted} />
      <TextInput
        accessibilityLabel={placeholder}
        value={value}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor="#83908C"
        returnKeyType="search"
        style={styles.searchInput}
      />
      {value ? (
        <Pressable accessibilityRole="button" accessibilityLabel="Limpar busca" onPress={() => onChangeText('')}>
          <Ionicons name="close-circle" size={21} color={colors.muted} />
        </Pressable>
      ) : null}
    </View>
  );
}

export function CategoryChip({
  item,
  active,
  onPress,
}: {
  item: Category;
  active?: boolean;
  onPress: () => void;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityState={{ selected: active }}
      onPress={onPress}
      style={({ pressed }) => [styles.category, active && styles.categoryActive, pressed && styles.pressed]}
    >
      <View style={[styles.categoryIcon, { backgroundColor: active ? colors.lime : item.color }]}>
        <Ionicons name={item.icon} size={22} color={colors.ink} />
      </View>
      <Text style={[styles.categoryText, active && styles.categoryTextActive]}>{item.label}</Text>
    </Pressable>
  );
}

export function ScoreBadge({ score }: { score: number }) {
  return (
    <View
      accessibilityLabel={`${score} por cento de acessibilidade`}
      style={[styles.score, score < 90 && styles.scoreAmber]}
    >
      <Ionicons name="accessibility" size={14} color={colors.forest} />
      <Text style={styles.scoreText}>{score}%</Text>
    </View>
  );
}

export function PlaceCard({
  place,
  favorite,
  onFavorite,
  onPress,
  horizontal = false,
}: {
  place: Place;
  favorite: boolean;
  onFavorite: () => void;
  onPress: () => void;
  horizontal?: boolean;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={`${place.name}, nota ${place.rating}, acessibilidade ${place.accessibilityScore} por cento`}
      onPress={onPress}
      style={({ pressed }) => [styles.placeCard, horizontal && styles.placeCardHorizontal, pressed && styles.cardPressed]}
    >
      <View style={[styles.placeVisual, { backgroundColor: place.color }]}>
        <View style={styles.placeIllustrationRing} />
        <Ionicons name={place.icon} size={48} color={colors.forest} />
        <View style={styles.placeScoreWrap}>
          <ScoreBadge score={place.accessibilityScore} />
        </View>
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={favorite ? `Remover ${place.name} dos favoritos` : `Adicionar ${place.name} aos favoritos`}
          onPress={(event) => {
            event.stopPropagation();
            onFavorite();
          }}
          hitSlop={10}
          style={styles.favorite}
        >
          <Ionicons name={favorite ? 'heart' : 'heart-outline'} size={21} color={favorite ? colors.coral : colors.ink} />
        </Pressable>
      </View>

      <View style={styles.placeBody}>
        <View style={styles.placeMetaRow}>
          <Text style={styles.placeCategory}>{place.category}</Text>
          {place.verified ? (
            <View style={styles.verified}>
              <Ionicons name="checkmark-circle" size={14} color={colors.green} />
              <Text style={styles.verifiedText}>Verificado</Text>
            </View>
          ) : null}
        </View>
        <Text style={styles.placeName} numberOfLines={1}>{place.name}</Text>
        <View style={styles.ratingRow}>
          <Ionicons name="star" size={16} color={colors.amber} />
          <Text style={styles.ratingStrong}>{place.rating}</Text>
          <Text style={styles.ratingMuted}>({place.reviews})</Text>
          <View style={styles.dot} />
          <Ionicons name="location-outline" size={15} color={colors.muted} />
          <Text style={styles.ratingMuted}>{place.distance}</Text>
        </View>
        <View style={styles.featureRow}>
          {place.features.slice(0, horizontal ? 2 : 3).map((feature) => (
            <View key={feature.label} style={styles.featurePill}>
              <Ionicons name={feature.icon} size={13} color={colors.green} />
              <Text style={styles.featureText} numberOfLines={1}>{feature.label}</Text>
            </View>
          ))}
        </View>
      </View>
    </Pressable>
  );
}

export function SectionHeading({
  title,
  action,
  onAction,
}: {
  title: string;
  action?: string;
  onAction?: () => void;
}) {
  return (
    <View style={styles.sectionHeading}>
      <Text style={styles.sectionTitle}>{title}</Text>
      {action ? (
        <Pressable accessibilityRole="button" onPress={onAction} hitSlop={8}>
          <Text style={styles.sectionAction}>{action}</Text>
        </Pressable>
      ) : null}
    </View>
  );
}

export function PrimaryButton({
  label,
  icon,
  onPress,
  disabled = false,
  light = false,
}: {
  label: string;
  icon?: IconName;
  onPress: () => void;
  disabled?: boolean;
  light?: boolean;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityState={{ disabled }}
      disabled={disabled}
      onPress={onPress}
      style={({ pressed }) => [
        styles.primaryButton,
        light && styles.primaryButtonLight,
        disabled && styles.primaryButtonDisabled,
        pressed && styles.pressed,
      ]}
    >
      {icon ? <Ionicons name={icon} size={19} color={light ? colors.forest : colors.surface} /> : null}
      <Text style={[styles.primaryButtonText, light && styles.primaryButtonTextLight]}>{label}</Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  pressed: { opacity: 0.72 },
  cardPressed: { opacity: 0.9, transform: [{ scale: 0.995 }] },
  iconButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: colors.surface,
    borderWidth: 1,
    borderColor: colors.line,
    alignItems: 'center',
    justifyContent: 'center',
  },
  iconButtonActive: { backgroundColor: colors.forest, borderColor: colors.forest },
  search: {
    minHeight: 54,
    borderRadius: radii.md,
    backgroundColor: colors.surface,
    borderColor: colors.line,
    borderWidth: 1,
    paddingHorizontal: 16,
    flexDirection: 'row',
    alignItems: 'center',
    gap: 10,
  },
  searchInput: { flex: 1, fontSize: 16, color: colors.ink, paddingVertical: 14 },
  category: { alignItems: 'center', width: 76, gap: 8 },
  categoryActive: {},
  categoryIcon: { width: 58, height: 58, borderRadius: 20, alignItems: 'center', justifyContent: 'center' },
  categoryText: { color: colors.muted, fontSize: 13, fontWeight: '600' },
  categoryTextActive: { color: colors.forest, fontWeight: '800' },
  score: {
    backgroundColor: colors.mint,
    borderRadius: radii.pill,
    paddingHorizontal: 9,
    paddingVertical: 6,
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
  },
  scoreAmber: { backgroundColor: colors.amberSoft },
  scoreText: { fontSize: 12, color: colors.forest, fontWeight: '900' },
  placeCard: {
    width: 286,
    backgroundColor: colors.surface,
    borderRadius: radii.lg,
    overflow: 'hidden',
    borderWidth: 1,
    borderColor: colors.line,
  },
  placeCardHorizontal: { width: '100%' },
  placeVisual: { height: 138, alignItems: 'center', justifyContent: 'center', overflow: 'hidden' },
  placeIllustrationRing: {
    position: 'absolute',
    width: 145,
    height: 145,
    borderRadius: 80,
    backgroundColor: 'rgba(255,255,255,0.34)',
    right: -22,
    bottom: -62,
  },
  placeScoreWrap: { position: 'absolute', top: 12, left: 12 },
  favorite: {
    position: 'absolute',
    top: 12,
    right: 12,
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: 'rgba(255,255,255,0.9)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  placeBody: { padding: 16 },
  placeMetaRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', gap: 8 },
  placeCategory: { fontSize: 12, fontWeight: '800', color: colors.green, textTransform: 'uppercase', letterSpacing: 0.5 },
  verified: { flexDirection: 'row', alignItems: 'center', gap: 3 },
  verifiedText: { fontSize: 11, fontWeight: '700', color: colors.green },
  placeName: { fontSize: 20, fontWeight: '900', color: colors.ink, marginTop: 5, letterSpacing: -0.3 },
  ratingRow: { flexDirection: 'row', alignItems: 'center', gap: 4, marginTop: 8 },
  ratingStrong: { fontSize: 14, color: colors.ink, fontWeight: '800' },
  ratingMuted: { fontSize: 13, color: colors.muted },
  dot: { width: 3, height: 3, borderRadius: 2, backgroundColor: '#9AABA6', marginHorizontal: 3 },
  featureRow: { flexDirection: 'row', gap: 6, marginTop: 13 },
  featurePill: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colors.cream,
    paddingVertical: 6,
    paddingHorizontal: 8,
    borderRadius: radii.pill,
    gap: 4,
    maxWidth: 112,
  },
  featureText: { color: colors.muted, fontSize: 10, fontWeight: '700' },
  sectionHeading: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  sectionTitle: { color: colors.ink, fontSize: 22, lineHeight: 28, fontWeight: '900', letterSpacing: -0.6 },
  sectionAction: { color: colors.green, fontSize: 14, fontWeight: '800' },
  primaryButton: {
    minHeight: 52,
    paddingHorizontal: 20,
    backgroundColor: colors.forest,
    borderRadius: radii.md,
    flexDirection: 'row',
    gap: 8,
    alignItems: 'center',
    justifyContent: 'center',
  },
  primaryButtonLight: { backgroundColor: colors.lime },
  primaryButtonDisabled: { opacity: 0.4 },
  primaryButtonText: { color: colors.surface, fontSize: 16, fontWeight: '900' },
  primaryButtonTextLight: { color: colors.forest },
});
