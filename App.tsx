import React, { useMemo, useState } from 'react';
import {
  KeyboardAvoidingView,
  Image,
  Modal,
  Platform,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  View,
  useWindowDimensions,
} from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { Ionicons } from '@expo/vector-icons';
import { SafeAreaProvider, SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import {
  CategoryChip,
  IconButton,
  PlaceCard,
  PrimaryButton,
  ScoreBadge,
  SearchField,
  SectionHeading,
} from './src/components';
import { categories, leaderboard, places } from './src/data';
import { colors, radii } from './src/theme';
import type { IconName, Place, TabId } from './src/types';

const tabs: { id: TabId; label: string; icon: IconName; activeIcon: IconName }[] = [
  { id: 'inicio', label: 'Início', icon: 'home-outline', activeIcon: 'home' },
  { id: 'explorar', label: 'Explorar', icon: 'compass-outline', activeIcon: 'compass' },
  { id: 'contribuir', label: 'Contribuir', icon: 'add', activeIcon: 'add' },
  { id: 'ranking', label: 'Ranking', icon: 'trophy-outline', activeIcon: 'trophy' },
  { id: 'perfil', label: 'Perfil', icon: 'person-outline', activeIcon: 'person' },
];

function AppContent() {
  const capturePreset = Platform.OS === 'web' && typeof window !== 'undefined'
    ? new URLSearchParams(window.location.search).get('capture')
    : null;
  const captureSize = capturePreset === 'google'
    ? { width: 390, height: 693 }
    : capturePreset === 'apple'
      ? { width: 430, height: 932 }
      : null;
  const [activeTab, setActiveTab] = useState<TabId>('inicio');
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState<string | null>(null);
  const [favorites, setFavorites] = useState<string[]>(['2']);
  const [detail, setDetail] = useState<Place | null>(null);
  const [contributeOpen, setContributeOpen] = useState(false);
  const [toast, setToast] = useState('');

  const toggleFavorite = (id: string) => {
    setFavorites((current) => (current.includes(id) ? current.filter((item) => item !== id) : [...current, id]));
  };

  const switchTab = (tab: TabId) => {
    if (tab === 'contribuir') {
      setContributeOpen(true);
      return;
    }
    setActiveTab(tab);
  };

  const notify = (message: string) => {
    setToast(message);
    setTimeout(() => setToast(''), 2800);
  };

  return (
    <View style={[styles.app, captureSize && styles.captureApp, captureSize]}>
      <StatusBar style="dark" />
      <View style={styles.contentFrame}>
        {activeTab === 'inicio' ? (
          <HomeScreen
            favorites={favorites}
            onFavorite={toggleFavorite}
            onPlace={setDetail}
            onExplore={() => setActiveTab('explorar')}
            onContribute={() => setContributeOpen(true)}
          />
        ) : null}
        {activeTab === 'explorar' ? (
          <ExploreScreen
            search={search}
            onSearch={setSearch}
            category={category}
            onCategory={setCategory}
            favorites={favorites}
            onFavorite={toggleFavorite}
            onPlace={setDetail}
          />
        ) : null}
        {activeTab === 'ranking' ? <RankingScreen /> : null}
        {activeTab === 'perfil' ? (
          <ProfileScreen favorites={favorites.length} onContribute={() => setContributeOpen(true)} />
        ) : null}
      </View>

      <BottomNav active={activeTab} onChange={switchTab} />
      <PlaceDetail
        place={detail}
        favorite={detail ? favorites.includes(detail.id) : false}
        onFavorite={() => detail && toggleFavorite(detail.id)}
        onClose={() => setDetail(null)}
        onReview={() => {
          setDetail(null);
          setContributeOpen(true);
        }}
      />
      <ContributionModal open={contributeOpen} onClose={() => setContributeOpen(false)} onSuccess={notify} />
      {toast ? (
        <View accessibilityLiveRegion="polite" style={styles.toast}>
          <Ionicons name="checkmark-circle" size={20} color={colors.lime} />
          <Text style={styles.toastText}>{toast}</Text>
        </View>
      ) : null}
    </View>
  );
}

function ScreenHeader({ title, subtitle }: { title: string; subtitle?: string }) {
  return (
    <View style={styles.screenHeader}>
      <View style={styles.flex}>
        <Text style={styles.eyebrow}>PCD DESTINO</Text>
        <Text style={styles.screenTitle}>{title}</Text>
        {subtitle ? <Text style={styles.screenSubtitle}>{subtitle}</Text> : null}
      </View>
      <Pressable accessibilityRole="button" accessibilityLabel="Abrir notificações" style={styles.notificationButton}>
        <Ionicons name="notifications-outline" size={21} color={colors.ink} />
        <View style={styles.notificationDot} />
      </Pressable>
    </View>
  );
}

function HomeScreen({
  favorites,
  onFavorite,
  onPlace,
  onExplore,
  onContribute,
}: {
  favorites: string[];
  onFavorite: (id: string) => void;
  onPlace: (place: Place) => void;
  onExplore: () => void;
  onContribute: () => void;
}) {
  return (
    <SafeAreaView edges={['top']} style={styles.screen}>
      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <View style={styles.locationHeader}>
          <View>
            <Text style={styles.greeting}>Olá, Alex 👋</Text>
            <Pressable accessibilityRole="button" style={styles.locationSelect}>
              <Ionicons name="location" size={16} color={colors.green} />
              <Text style={styles.locationText}>Campinas, SP</Text>
              <Ionicons name="chevron-down" size={15} color={colors.muted} />
            </Pressable>
          </View>
          <View style={styles.avatar} accessibilityLabel="Perfil de Alex">
            <Text style={styles.avatarText}>AL</Text>
          </View>
        </View>

        <View style={styles.hero}>
          <View style={styles.heroGlow} />
          <View style={styles.heroIcon}>
            <Image source={require('./assets/brand/logo-master.png')} style={styles.heroLogo} accessibilityLabel="Logotipo PCD Destino" />
          </View>
          <Text style={styles.heroKicker}>CIDADE PARA TODOS</Text>
          <Text style={styles.heroTitle}>Acessibilidade que conecta.</Text>
          <Text style={styles.heroText}>Descubra e compartilhe lugares onde todo mundo é bem-vindo.</Text>
          <View style={styles.heroButtonWrap}>
            <PrimaryButton label="Explorar agora" icon="arrow-forward" light onPress={onExplore} />
          </View>
        </View>

        <Pressable accessibilityRole="search" onPress={onExplore}>
          <View pointerEvents="none">
            <SearchField value="" onChangeText={() => undefined} />
          </View>
        </Pressable>

        <SectionHeading title="O que você procura?" />
        <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.categoryList}>
          {categories.map((item) => (
            <CategoryChip key={item.id} item={item} onPress={onExplore} />
          ))}
        </ScrollView>

        <SectionHeading title="Destaques perto de você" action="Ver todos" onAction={onExplore} />
        <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.cardsRow}>
          {places.map((place) => (
            <PlaceCard
              key={place.id}
              place={place}
              favorite={favorites.includes(place.id)}
              onFavorite={() => onFavorite(place.id)}
              onPress={() => onPlace(place)}
            />
          ))}
        </ScrollView>

        <View style={styles.communityCard}>
          <View style={styles.communityIcon}>
            <Ionicons name="people" size={26} color={colors.forest} />
          </View>
          <View style={styles.communityCopy}>
            <Text style={styles.communityTitle}>Sua experiência importa</Text>
            <Text style={styles.communityText}>Avalie um local, ajude a comunidade e ganhe pontos.</Text>
          </View>
          <IconButton icon="arrow-forward" label="Contribuir" onPress={onContribute} active />
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

function ExploreScreen({
  search,
  onSearch,
  category,
  onCategory,
  favorites,
  onFavorite,
  onPlace,
}: {
  search: string;
  onSearch: (text: string) => void;
  category: string | null;
  onCategory: (id: string | null) => void;
  favorites: string[];
  onFavorite: (id: string) => void;
  onPlace: (place: Place) => void;
}) {
  const results = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('pt-BR');
    return places.filter((place) => {
      const matchesText = !term || `${place.name} ${place.category} ${place.neighborhood}`.toLocaleLowerCase('pt-BR').includes(term);
      const matchesCategory = !category || place.category.toLocaleLowerCase('pt-BR').includes(category === 'lazer' ? 'lazer' : category === 'esporte' ? 'esporte' : '');
      return matchesText && matchesCategory;
    });
  }, [search, category]);

  return (
    <SafeAreaView edges={['top']} style={styles.screen}>
      <ScrollView keyboardShouldPersistTaps="handled" showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <ScreenHeader title="Explore sua cidade" subtitle="Lugares avaliados por quem vive a acessibilidade." />
        <SearchField value={search} onChangeText={onSearch} />
        <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.filterRow}>
          <Pressable
            accessibilityRole="button"
            accessibilityState={{ selected: category === null }}
            onPress={() => onCategory(null)}
            style={[styles.filterChip, category === null && styles.filterChipActive]}
          >
            <Text style={[styles.filterText, category === null && styles.filterTextActive]}>Todos</Text>
          </Pressable>
          {categories.map((item) => (
            <Pressable
              key={item.id}
              accessibilityRole="button"
              accessibilityState={{ selected: category === item.id }}
              onPress={() => onCategory(item.id)}
              style={[styles.filterChip, category === item.id && styles.filterChipActive]}
            >
              <Ionicons name={item.icon} size={16} color={category === item.id ? colors.surface : colors.green} />
              <Text style={[styles.filterText, category === item.id && styles.filterTextActive]}>{item.label}</Text>
            </Pressable>
          ))}
        </ScrollView>

        <View style={styles.resultsHeader}>
          <Text accessibilityLiveRegion="polite" style={styles.resultsText}>{results.length} lugares encontrados</Text>
          <Pressable accessibilityRole="button" style={styles.sortButton}>
            <Ionicons name="options-outline" size={17} color={colors.green} />
            <Text style={styles.sortText}>Filtrar</Text>
          </Pressable>
        </View>

        <View style={styles.placeList}>
          {results.map((place) => (
            <PlaceCard
              key={place.id}
              place={place}
              horizontal
              favorite={favorites.includes(place.id)}
              onFavorite={() => onFavorite(place.id)}
              onPress={() => onPlace(place)}
            />
          ))}
          {results.length === 0 ? (
            <View style={styles.emptyState}>
              <View style={styles.emptyIcon}><Ionicons name="search-outline" size={32} color={colors.green} /></View>
              <Text style={styles.emptyTitle}>Nada por aqui ainda</Text>
              <Text style={styles.emptyText}>Tente outro termo ou remova os filtros.</Text>
            </View>
          ) : null}
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

function RankingScreen() {
  return (
    <SafeAreaView edges={['top']} style={styles.screen}>
      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <ScreenHeader title="Comunidade que transforma" subtitle="Cada contribuição torna a cidade mais inclusiva." />
        <View style={styles.pointsHero}>
          <View>
            <Text style={styles.pointsLabel}>SEUS PONTOS</Text>
            <Text style={styles.pointsValue}>1.480</Text>
            <Text style={styles.pointsPosition}>Você está em 8º na sua cidade</Text>
          </View>
          <View style={styles.levelBadge}>
            <Ionicons name="ribbon" size={32} color={colors.forest} />
            <Text style={styles.levelText}>Nível 6</Text>
          </View>
        </View>

        <View style={styles.progressCard}>
          <View style={styles.progressTop}>
            <Text style={styles.progressTitle}>Rumo ao nível 7</Text>
            <Text style={styles.progressValue}>520 pts</Text>
          </View>
          <View style={styles.progressTrack}><View style={styles.progressFill} /></View>
          <Text style={styles.progressCaption}>Mais 2 avaliações completas e você chega lá!</Text>
        </View>

        <SectionHeading title="Ranking da cidade" action="Este mês" />
        <View style={styles.leaderboardCard}>
          {leaderboard.map((person, index) => (
            <View key={person.name} style={[styles.leaderRow, index < leaderboard.length - 1 && styles.leaderDivider]}>
              <Text style={[styles.rankNumber, index < 3 && styles.rankTop]}>{index + 1}</Text>
              <View style={[styles.leaderAvatar, { backgroundColor: person.color }]}>
                <Text style={styles.leaderInitials}>{person.initials}</Text>
              </View>
              <View style={styles.flex}>
                <Text style={styles.leaderName}>{person.name}</Text>
                <Text style={styles.leaderReviews}>{person.reviews} contribuições</Text>
              </View>
              <View style={styles.leaderPointsWrap}>
                <Ionicons name="sparkles" size={14} color={colors.amber} />
                <Text style={styles.leaderPoints}>{person.points.toLocaleString('pt-BR')}</Text>
              </View>
            </View>
          ))}
          <View style={[styles.leaderRow, styles.youRow]}>
            <Text style={styles.rankNumber}>8</Text>
            <View style={[styles.leaderAvatar, { backgroundColor: colors.forest }]}><Text style={styles.leaderInitials}>AL</Text></View>
            <View style={styles.flex}>
              <Text style={styles.leaderName}>Você</Text>
              <Text style={styles.leaderReviews}>21 contribuições</Text>
            </View>
            <Text style={styles.leaderPoints}>1.480</Text>
          </View>
        </View>

        <SectionHeading title="Conquistas recentes" />
        <View style={styles.achievementRow}>
          <Achievement icon="map" title="Explorador" detail="10 locais" />
          <Achievement icon="heart" title="Aliado PCD" detail="20 avaliações" />
          <Achievement icon="star" title="Referência" detail="Top 10" />
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

function Achievement({ icon, title, detail }: { icon: IconName; title: string; detail: string }) {
  return (
    <View style={styles.achievement}>
      <View style={styles.achievementIcon}><Ionicons name={icon} size={24} color={colors.forest} /></View>
      <Text style={styles.achievementTitle}>{title}</Text>
      <Text style={styles.achievementDetail}>{detail}</Text>
    </View>
  );
}

function ProfileScreen({ favorites, onContribute }: { favorites: number; onContribute: () => void }) {
  const menu: { label: string; icon: IconName; detail?: string }[] = [
    { label: 'Minhas contribuições', icon: 'chatbubbles-outline', detail: '21' },
    { label: 'Locais favoritos', icon: 'heart-outline', detail: String(favorites) },
    { label: 'Preferências de acessibilidade', icon: 'accessibility-outline' },
    { label: 'Cidade e localização', icon: 'location-outline', detail: 'Campinas' },
    { label: 'Ajuda e segurança', icon: 'shield-checkmark-outline' },
  ];
  return (
    <SafeAreaView edges={['top']} style={styles.screen}>
      <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.scrollContent}>
        <ScreenHeader title="Seu perfil" />
        <View style={styles.profileCard}>
          <View style={styles.profileAvatar}><Text style={styles.profileInitials}>AL</Text></View>
          <Text style={styles.profileName}>Alex Lima</Text>
          <Text style={styles.profileHandle}>@alexacessivel</Text>
          <View style={styles.profileStats}>
            <View style={styles.profileStat}><Text style={styles.profileStatValue}>1.480</Text><Text style={styles.profileStatLabel}>pontos</Text></View>
            <View style={styles.statDivider} />
            <View style={styles.profileStat}><Text style={styles.profileStatValue}>21</Text><Text style={styles.profileStatLabel}>contribuições</Text></View>
            <View style={styles.statDivider} />
            <View style={styles.profileStat}><Text style={styles.profileStatValue}>6</Text><Text style={styles.profileStatLabel}>nível</Text></View>
          </View>
        </View>
        <PrimaryButton label="Fazer nova contribuição" icon="add-circle-outline" onPress={onContribute} />
        <View style={styles.menuCard}>
          {menu.map((item, index) => (
            <Pressable key={item.label} accessibilityRole="button" style={[styles.menuRow, index < menu.length - 1 && styles.menuDivider]}>
              <View style={styles.menuIcon}><Ionicons name={item.icon} size={20} color={colors.green} /></View>
              <Text style={styles.menuLabel}>{item.label}</Text>
              {item.detail ? <Text style={styles.menuDetail}>{item.detail}</Text> : null}
              <Ionicons name="chevron-forward" size={18} color="#8FA09B" />
            </Pressable>
          ))}
        </View>
        <Text style={styles.version}>PCD Destino • versão 1.0.0</Text>
      </ScrollView>
    </SafeAreaView>
  );
}

function BottomNav({ active, onChange }: { active: TabId; onChange: (tab: TabId) => void }) {
  const insets = useSafeAreaInsets();
  return (
    <View style={[styles.bottomNavOuter, { paddingBottom: Math.max(insets.bottom, 8) }]}>
      <View style={styles.bottomNav}>
        {tabs.map((tab) => {
          const selected = active === tab.id;
          const isAdd = tab.id === 'contribuir';
          return (
            <Pressable
              key={tab.id}
              accessibilityRole="tab"
              accessibilityState={{ selected }}
              accessibilityLabel={tab.label}
              onPress={() => onChange(tab.id)}
              style={styles.tab}
            >
              <View style={isAdd ? styles.addTabIcon : styles.tabIcon}>
                <Ionicons name={selected ? tab.activeIcon : tab.icon} size={isAdd ? 28 : 22} color={isAdd ? colors.surface : selected ? colors.forest : '#7D8C88'} />
              </View>
              <Text style={[styles.tabLabel, selected && styles.tabLabelActive, isAdd && styles.addTabLabel]}>{tab.label}</Text>
            </Pressable>
          );
        })}
      </View>
    </View>
  );
}

function PlaceDetail({
  place,
  favorite,
  onFavorite,
  onClose,
  onReview,
}: {
  place: Place | null;
  favorite: boolean;
  onFavorite: () => void;
  onClose: () => void;
  onReview: () => void;
}) {
  if (!place) return null;
  return (
    <Modal visible animationType="slide" presentationStyle="pageSheet" onRequestClose={onClose}>
      <SafeAreaView style={styles.modalScreen}>
        <View style={styles.modalHeader}>
          <IconButton icon="close" label="Fechar detalhes" onPress={onClose} />
          <Text style={styles.modalHeaderTitle}>Detalhes do local</Text>
          <IconButton icon={favorite ? 'heart' : 'heart-outline'} label="Favoritar local" onPress={onFavorite} />
        </View>
        <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.detailContent}>
          <View style={[styles.detailVisual, { backgroundColor: place.color }]}>
            <View style={styles.detailVisualCircle} />
            <Ionicons name={place.icon} size={76} color={colors.forest} />
          </View>
          <View style={styles.detailTitleRow}>
            <View style={styles.flex}>
              <Text style={styles.placeCategory}>{place.category}</Text>
              <Text style={styles.detailTitle}>{place.name}</Text>
              <View style={styles.ratingRow}>
                <Ionicons name="star" size={17} color={colors.amber} />
                <Text style={styles.ratingStrong}>{place.rating}</Text>
                <Text style={styles.ratingMuted}>{place.reviews} avaliações</Text>
                <View style={styles.dot} />
                <Text style={styles.ratingMuted}>{place.neighborhood}</Text>
              </View>
            </View>
            <ScoreBadge score={place.accessibilityScore} />
          </View>
          <Text style={styles.detailDescription}>{place.description}</Text>

          <SectionHeading title="Recursos de acessibilidade" />
          <View style={styles.detailFeatures}>
            {place.features.map((feature) => (
              <View key={feature.label} style={styles.detailFeature}>
                <View style={styles.detailFeatureIcon}><Ionicons name={feature.icon} size={22} color={colors.green} /></View>
                <Text style={styles.detailFeatureText}>{feature.label}</Text>
                <Ionicons name="checkmark-circle" size={20} color={colors.green} />
              </View>
            ))}
          </View>

          <View style={styles.infoCard}>
            <Ionicons name="information-circle-outline" size={23} color={colors.blue} />
            <Text style={styles.infoText}>Informações colaborativas. Confirme diretamente com o local antes da visita.</Text>
          </View>
          <PrimaryButton label="Como chegar" icon="navigate-outline" onPress={() => undefined} />
          <PrimaryButton label="Avaliar este local" icon="star-outline" light onPress={onReview} />
        </ScrollView>
      </SafeAreaView>
    </Modal>
  );
}

function ContributionModal({
  open,
  onClose,
  onSuccess,
}: {
  open: boolean;
  onClose: () => void;
  onSuccess: (message: string) => void;
}) {
  const [step, setStep] = useState(1);
  const [kind, setKind] = useState<'avaliar' | 'cadastrar'>('avaliar');
  const [placeName, setPlaceName] = useState('');
  const [rating, setRating] = useState(0);
  const [features, setFeatures] = useState<string[]>([]);
  const [comment, setComment] = useState('');

  const close = () => {
    setStep(1);
    onClose();
  };
  const submit = () => {
    close();
    setPlaceName('');
    setRating(0);
    setFeatures([]);
    setComment('');
    onSuccess('Contribuição enviada! Você ganhou 80 pontos.');
  };
  const featureOptions: { label: string; icon: IconName }[] = [
    { label: 'Entrada sem degraus', icon: 'remove-outline' },
    { label: 'Banheiro adaptado', icon: 'accessibility-outline' },
    { label: 'Piso tátil', icon: 'trail-sign-outline' },
    { label: 'Intérprete de Libras', icon: 'hand-left-outline' },
    { label: 'Vaga reservada', icon: 'car-outline' },
    { label: 'Cardápio acessível', icon: 'reader-outline' },
  ];

  return (
    <Modal visible={open} animationType="slide" presentationStyle="pageSheet" onRequestClose={close}>
      <SafeAreaView style={styles.modalScreen}>
        <KeyboardAvoidingView style={styles.flex} behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
          <View style={styles.modalHeader}>
            <IconButton icon="close" label="Fechar contribuição" onPress={close} />
            <View style={styles.stepDots}>
              {[1, 2, 3].map((item) => <View key={item} style={[styles.stepDot, item <= step && styles.stepDotActive]} />)}
            </View>
            <Text style={styles.stepText}>{step}/3</Text>
          </View>
          <ScrollView keyboardShouldPersistTaps="handled" showsVerticalScrollIndicator={false} contentContainerStyle={styles.contributionContent}>
            {step === 1 ? (
              <>
                <Text style={styles.eyebrow}>CONTRIBUA COM A COMUNIDADE</Text>
                <Text style={styles.contributionTitle}>O que você quer compartilhar?</Text>
                <Text style={styles.contributionSubtitle}>Sua experiência ajuda outras pessoas a planejar com segurança.</Text>
                <View style={styles.kindRow}>
                  <SelectCard
                    selected={kind === 'avaliar'}
                    icon="star-outline"
                    title="Avaliar um local"
                    detail="Conte como foi sua experiência"
                    onPress={() => setKind('avaliar')}
                  />
                  <SelectCard
                    selected={kind === 'cadastrar'}
                    icon="business-outline"
                    title="Cadastrar local"
                    detail="Adicione um novo ponto acessível"
                    onPress={() => setKind('cadastrar')}
                  />
                </View>
                <Text style={styles.inputLabel}>Nome do local ou serviço</Text>
                <View style={styles.textFieldWrap}>
                  <Ionicons name="search-outline" size={20} color={colors.muted} />
                  <TextInput
                    value={placeName}
                    onChangeText={setPlaceName}
                    placeholder="Ex.: Parque da Cidade"
                    placeholderTextColor="#83908C"
                    style={styles.textField}
                  />
                </View>
              </>
            ) : null}

            {step === 2 ? (
              <>
                <Text style={styles.eyebrow}>ACESSIBILIDADE NA PRÁTICA</Text>
                <Text style={styles.contributionTitle}>Como foi sua experiência?</Text>
                <Text style={styles.contributionSubtitle}>Avalie pensando na sua autonomia, conforto e segurança.</Text>
                <Text style={styles.inputLabel}>Nota geral de acessibilidade</Text>
                <View accessibilityRole="radiogroup" style={styles.starRow}>
                  {[1, 2, 3, 4, 5].map((value) => (
                    <Pressable
                      key={value}
                      accessibilityRole="radio"
                      accessibilityState={{ checked: rating === value }}
                      accessibilityLabel={`${value} estrelas`}
                      onPress={() => setRating(value)}
                      hitSlop={6}
                    >
                      <Ionicons name={rating >= value ? 'star' : 'star-outline'} size={38} color={colors.amber} />
                    </Pressable>
                  ))}
                </View>
                <Text style={styles.inputLabel}>Quais recursos você encontrou?</Text>
                <View style={styles.featureGrid}>
                  {featureOptions.map((option) => {
                    const selected = features.includes(option.label);
                    return (
                      <Pressable
                        key={option.label}
                        accessibilityRole="checkbox"
                        accessibilityState={{ checked: selected }}
                        onPress={() => setFeatures((current) => selected ? current.filter((item) => item !== option.label) : [...current, option.label])}
                        style={[styles.optionChip, selected && styles.optionChipActive]}
                      >
                        <Ionicons name={selected ? 'checkmark-circle' : option.icon} size={19} color={selected ? colors.surface : colors.green} />
                        <Text style={[styles.optionText, selected && styles.optionTextActive]}>{option.label}</Text>
                      </Pressable>
                    );
                  })}
                </View>
              </>
            ) : null}

            {step === 3 ? (
              <>
                <Text style={styles.eyebrow}>ÚLTIMOS DETALHES</Text>
                <Text style={styles.contributionTitle}>Conte um pouco mais</Text>
                <Text style={styles.contributionSubtitle}>Uma dica objetiva pode fazer toda a diferença para a próxima pessoa.</Text>
                <Text style={styles.inputLabel}>Comentário</Text>
                <TextInput
                  value={comment}
                  onChangeText={setComment}
                  placeholder="Como foi o atendimento? Há alguma barreira importante?"
                  placeholderTextColor="#83908C"
                  multiline
                  textAlignVertical="top"
                  style={styles.commentField}
                />
                <View style={styles.rewardCard}>
                  <View style={styles.rewardIcon}><Ionicons name="sparkles" size={28} color={colors.forest} /></View>
                  <View style={styles.flex}>
                    <Text style={styles.rewardTitle}>+80 pontos</Text>
                    <Text style={styles.rewardText}>por uma avaliação completa</Text>
                  </View>
                  <Ionicons name="trophy-outline" size={27} color={colors.amber} />
                </View>
                <View style={styles.reviewSummary}>
                  <Text style={styles.reviewSummaryTitle}>Resumo</Text>
                  <SummaryRow label="Local" value={placeName || 'Local selecionado'} />
                  <SummaryRow label="Avaliação" value={`${rating} de 5 estrelas`} />
                  <SummaryRow label="Recursos" value={`${features.length} selecionados`} />
                </View>
              </>
            ) : null}
          </ScrollView>
          <View style={styles.contributionFooter}>
            {step > 1 ? <IconButton icon="arrow-back" label="Voltar" onPress={() => setStep(step - 1)} /> : null}
            <View style={styles.flex}>
              <PrimaryButton
                label={step === 3 ? 'Enviar contribuição' : 'Continuar'}
                icon={step === 3 ? 'checkmark-circle-outline' : 'arrow-forward'}
                disabled={(step === 1 && !placeName.trim()) || (step === 2 && rating === 0)}
                onPress={() => step === 3 ? submit() : setStep(step + 1)}
              />
            </View>
          </View>
        </KeyboardAvoidingView>
      </SafeAreaView>
    </Modal>
  );
}

function SelectCard({ selected, icon, title, detail, onPress }: { selected: boolean; icon: IconName; title: string; detail: string; onPress: () => void }) {
  return (
    <Pressable
      accessibilityRole="radio"
      accessibilityState={{ checked: selected }}
      onPress={onPress}
      style={[styles.selectCard, selected && styles.selectCardActive]}
    >
      <View style={[styles.selectIcon, selected && styles.selectIconActive]}>
        <Ionicons name={icon} size={25} color={selected ? colors.surface : colors.green} />
      </View>
      <Text style={styles.selectTitle}>{title}</Text>
      <Text style={styles.selectDetail}>{detail}</Text>
      <Ionicons name={selected ? 'checkmark-circle' : 'ellipse-outline'} size={22} color={selected ? colors.green : '#9AABA6'} />
    </Pressable>
  );
}

function SummaryRow({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.summaryRow}>
      <Text style={styles.summaryLabel}>{label}</Text>
      <Text style={styles.summaryValue}>{value}</Text>
    </View>
  );
}

export default function App() {
  return (
    <SafeAreaProvider>
      <AppContent />
    </SafeAreaProvider>
  );
}

const styles = StyleSheet.create({
  app: { flex: 1, backgroundColor: colors.cream, alignItems: 'center' },
  captureApp: { flexGrow: 0, flexShrink: 0, flexBasis: 'auto', alignSelf: 'center', overflow: 'hidden' },
  contentFrame: { flex: 1, width: '100%', maxWidth: 720 },
  flex: { flex: 1 },
  screen: { flex: 1, backgroundColor: colors.cream },
  scrollContent: { paddingHorizontal: 20, paddingBottom: 34, gap: 22 },
  locationHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingTop: 4 },
  greeting: { fontSize: 25, fontWeight: '900', color: colors.ink, letterSpacing: -0.6 },
  locationSelect: { flexDirection: 'row', alignItems: 'center', gap: 4, marginTop: 5, minHeight: 28 },
  locationText: { color: colors.muted, fontSize: 14, fontWeight: '700' },
  avatar: { width: 48, height: 48, borderRadius: 18, backgroundColor: colors.lime, alignItems: 'center', justifyContent: 'center' },
  avatarText: { fontWeight: '900', color: colors.forest, fontSize: 15 },
  hero: { backgroundColor: colors.forest, borderRadius: 30, padding: 24, minHeight: 300, overflow: 'hidden', justifyContent: 'flex-end' },
  heroGlow: { position: 'absolute', width: 260, height: 260, borderRadius: 130, backgroundColor: '#2B5C4E', top: -95, right: -75 },
  heroIcon: { position: 'absolute', right: 28, top: 30, width: 58, height: 58, borderRadius: 20, backgroundColor: colors.lime, alignItems: 'center', justifyContent: 'center', transform: [{ rotate: '6deg' }] },
  heroLogo: { width: 58, height: 58, borderRadius: 20 },
  heroKicker: { color: colors.lime, fontWeight: '900', fontSize: 12, letterSpacing: 1.3, marginBottom: 9 },
  heroTitle: { color: colors.surface, fontWeight: '900', fontSize: 34, lineHeight: 37, maxWidth: 310, letterSpacing: -1.2 },
  heroText: { color: '#C5D8D1', fontSize: 15, lineHeight: 22, marginTop: 11, maxWidth: 330 },
  heroButtonWrap: { marginTop: 20, alignSelf: 'flex-start' },
  categoryList: { gap: 12, paddingRight: 8 },
  cardsRow: { gap: 14, paddingRight: 8 },
  communityCard: { flexDirection: 'row', alignItems: 'center', padding: 16, borderRadius: radii.lg, backgroundColor: colors.mint, gap: 12 },
  communityIcon: { width: 46, height: 46, borderRadius: 16, backgroundColor: colors.lime, alignItems: 'center', justifyContent: 'center' },
  communityCopy: { flex: 1 },
  communityTitle: { color: colors.ink, fontWeight: '900', fontSize: 16 },
  communityText: { color: colors.muted, fontSize: 12, lineHeight: 17, marginTop: 2 },
  screenHeader: { flexDirection: 'row', alignItems: 'flex-start', gap: 12, paddingTop: 4 },
  eyebrow: { fontSize: 11, lineHeight: 16, color: colors.green, fontWeight: '900', letterSpacing: 1.3 },
  screenTitle: { fontSize: 30, lineHeight: 35, color: colors.ink, fontWeight: '900', letterSpacing: -1 },
  screenSubtitle: { color: colors.muted, fontSize: 14, lineHeight: 20, marginTop: 5, maxWidth: 420 },
  notificationButton: { width: 44, height: 44, borderRadius: 18, borderWidth: 1, borderColor: colors.line, backgroundColor: colors.surface, alignItems: 'center', justifyContent: 'center' },
  notificationDot: { position: 'absolute', width: 8, height: 8, borderRadius: 4, backgroundColor: colors.coral, right: 10, top: 9, borderWidth: 1.5, borderColor: colors.surface },
  filterRow: { gap: 8, paddingRight: 8 },
  filterChip: { minHeight: 40, borderRadius: radii.pill, backgroundColor: colors.surface, borderWidth: 1, borderColor: colors.line, paddingHorizontal: 15, flexDirection: 'row', alignItems: 'center', gap: 6 },
  filterChipActive: { backgroundColor: colors.forest, borderColor: colors.forest },
  filterText: { color: colors.muted, fontWeight: '800', fontSize: 13 },
  filterTextActive: { color: colors.surface },
  resultsHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  resultsText: { color: colors.ink, fontWeight: '800', fontSize: 15 },
  sortButton: { flexDirection: 'row', alignItems: 'center', gap: 5, minHeight: 38 },
  sortText: { color: colors.green, fontWeight: '800', fontSize: 14 },
  placeList: { gap: 16 },
  emptyState: { alignItems: 'center', paddingVertical: 50 },
  emptyIcon: { width: 66, height: 66, borderRadius: 24, backgroundColor: colors.mint, alignItems: 'center', justifyContent: 'center' },
  emptyTitle: { marginTop: 16, color: colors.ink, fontSize: 20, fontWeight: '900' },
  emptyText: { marginTop: 5, color: colors.muted, fontSize: 14 },
  pointsHero: { backgroundColor: colors.forest, borderRadius: radii.lg, padding: 22, flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  pointsLabel: { color: colors.lime, fontSize: 11, fontWeight: '900', letterSpacing: 1.2 },
  pointsValue: { color: colors.surface, fontSize: 38, fontWeight: '900', letterSpacing: -1 },
  pointsPosition: { color: '#C5D8D1', fontSize: 12, marginTop: 2 },
  levelBadge: { backgroundColor: colors.lime, width: 82, height: 82, borderRadius: 28, alignItems: 'center', justifyContent: 'center' },
  levelText: { color: colors.forest, fontSize: 12, fontWeight: '900', marginTop: 2 },
  progressCard: { backgroundColor: colors.surface, borderRadius: radii.md, borderWidth: 1, borderColor: colors.line, padding: 17 },
  progressTop: { flexDirection: 'row', justifyContent: 'space-between' },
  progressTitle: { color: colors.ink, fontWeight: '900', fontSize: 15 },
  progressValue: { color: colors.green, fontWeight: '900', fontSize: 13 },
  progressTrack: { height: 9, backgroundColor: colors.line, borderRadius: 6, marginTop: 12, overflow: 'hidden' },
  progressFill: { width: '68%', height: '100%', backgroundColor: colors.lime, borderRadius: 6 },
  progressCaption: { color: colors.muted, fontSize: 12, marginTop: 9 },
  leaderboardCard: { backgroundColor: colors.surface, borderRadius: radii.lg, borderWidth: 1, borderColor: colors.line, overflow: 'hidden' },
  leaderRow: { flexDirection: 'row', alignItems: 'center', paddingHorizontal: 15, paddingVertical: 14, gap: 11 },
  leaderDivider: { borderBottomWidth: 1, borderBottomColor: colors.line },
  rankNumber: { width: 22, color: colors.muted, fontWeight: '900', fontSize: 15, textAlign: 'center' },
  rankTop: { color: colors.amber, fontSize: 18 },
  leaderAvatar: { width: 42, height: 42, borderRadius: 15, alignItems: 'center', justifyContent: 'center' },
  leaderInitials: { color: colors.surface, fontWeight: '900', fontSize: 13 },
  leaderName: { color: colors.ink, fontWeight: '900', fontSize: 14 },
  leaderReviews: { color: colors.muted, fontSize: 11, marginTop: 2 },
  leaderPointsWrap: { flexDirection: 'row', gap: 4, alignItems: 'center' },
  leaderPoints: { color: colors.ink, fontWeight: '900', fontSize: 14 },
  youRow: { backgroundColor: colors.mint, margin: 8, borderRadius: 16 },
  achievementRow: { flexDirection: 'row', gap: 9 },
  achievement: { flex: 1, alignItems: 'center', padding: 12, borderRadius: radii.md, backgroundColor: colors.surface, borderWidth: 1, borderColor: colors.line },
  achievementIcon: { width: 43, height: 43, borderRadius: 15, backgroundColor: colors.mint, alignItems: 'center', justifyContent: 'center' },
  achievementTitle: { color: colors.ink, fontWeight: '900', fontSize: 12, marginTop: 8, textAlign: 'center' },
  achievementDetail: { color: colors.muted, fontSize: 10, marginTop: 2, textAlign: 'center' },
  profileCard: { alignItems: 'center', backgroundColor: colors.surface, borderRadius: radii.lg, borderWidth: 1, borderColor: colors.line, padding: 22 },
  profileAvatar: { width: 76, height: 76, borderRadius: 28, backgroundColor: colors.lime, alignItems: 'center', justifyContent: 'center' },
  profileInitials: { color: colors.forest, fontSize: 23, fontWeight: '900' },
  profileName: { color: colors.ink, fontSize: 23, fontWeight: '900', marginTop: 11 },
  profileHandle: { color: colors.muted, fontSize: 13, marginTop: 3 },
  profileStats: { flexDirection: 'row', alignItems: 'center', width: '100%', marginTop: 21, backgroundColor: colors.cream, borderRadius: radii.md, paddingVertical: 13 },
  profileStat: { flex: 1, alignItems: 'center' },
  profileStatValue: { color: colors.ink, fontSize: 18, fontWeight: '900' },
  profileStatLabel: { color: colors.muted, fontSize: 10, marginTop: 2 },
  statDivider: { width: 1, height: 28, backgroundColor: colors.line },
  menuCard: { backgroundColor: colors.surface, borderRadius: radii.lg, borderWidth: 1, borderColor: colors.line, overflow: 'hidden' },
  menuRow: { flexDirection: 'row', alignItems: 'center', gap: 12, padding: 15, minHeight: 62 },
  menuDivider: { borderBottomWidth: 1, borderBottomColor: colors.line },
  menuIcon: { width: 34, height: 34, borderRadius: 12, backgroundColor: colors.mint, alignItems: 'center', justifyContent: 'center' },
  menuLabel: { flex: 1, color: colors.ink, fontWeight: '800', fontSize: 14 },
  menuDetail: { color: colors.muted, fontSize: 12 },
  version: { textAlign: 'center', color: '#8FA09B', fontSize: 11 },
  bottomNavOuter: { width: '100%', maxWidth: 720, backgroundColor: colors.surface, borderTopWidth: 1, borderTopColor: colors.line },
  bottomNav: { height: 67, flexDirection: 'row', alignItems: 'center', paddingHorizontal: 5 },
  tab: { flex: 1, alignItems: 'center', justifyContent: 'center', minHeight: 58 },
  tabIcon: { height: 28, alignItems: 'center', justifyContent: 'center' },
  addTabIcon: { width: 52, height: 52, borderRadius: 20, backgroundColor: colors.forest, alignItems: 'center', justifyContent: 'center', marginTop: -28, borderWidth: 4, borderColor: colors.surface },
  tabLabel: { color: '#7D8C88', fontSize: 10, fontWeight: '700', marginTop: 2 },
  tabLabelActive: { color: colors.forest, fontWeight: '900' },
  addTabLabel: { marginTop: 0 },
  toast: { position: 'absolute', bottom: 92, alignSelf: 'center', backgroundColor: colors.forest, borderRadius: radii.pill, paddingVertical: 12, paddingHorizontal: 17, flexDirection: 'row', gap: 8, alignItems: 'center', maxWidth: '90%' },
  toastText: { color: colors.surface, fontSize: 13, fontWeight: '800' },
  modalScreen: { flex: 1, backgroundColor: colors.cream },
  modalHeader: { minHeight: 68, paddingHorizontal: 18, flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', borderBottomWidth: 1, borderBottomColor: colors.line, backgroundColor: colors.cream },
  modalHeaderTitle: { color: colors.ink, fontSize: 15, fontWeight: '900' },
  detailContent: { padding: 20, paddingBottom: 42, gap: 18, maxWidth: 720, width: '100%', alignSelf: 'center' },
  detailVisual: { height: 220, borderRadius: 28, alignItems: 'center', justifyContent: 'center', overflow: 'hidden' },
  detailVisualCircle: { position: 'absolute', width: 280, height: 280, borderRadius: 140, backgroundColor: 'rgba(255,255,255,0.32)', right: -45, bottom: -120 },
  detailTitleRow: { flexDirection: 'row', alignItems: 'flex-start', gap: 12 },
  placeCategory: { fontSize: 12, fontWeight: '900', color: colors.green, textTransform: 'uppercase', letterSpacing: 0.5 },
  detailTitle: { color: colors.ink, fontSize: 28, lineHeight: 33, fontWeight: '900', letterSpacing: -0.8, marginTop: 4 },
  ratingRow: { flexDirection: 'row', alignItems: 'center', gap: 5, marginTop: 7 },
  ratingStrong: { color: colors.ink, fontSize: 14, fontWeight: '900' },
  ratingMuted: { color: colors.muted, fontSize: 13 },
  dot: { width: 3, height: 3, borderRadius: 2, backgroundColor: '#9AABA6', marginHorizontal: 2 },
  detailDescription: { color: colors.muted, fontSize: 15, lineHeight: 23 },
  detailFeatures: { gap: 9 },
  detailFeature: { flexDirection: 'row', alignItems: 'center', gap: 12, padding: 13, borderRadius: radii.md, backgroundColor: colors.surface, borderWidth: 1, borderColor: colors.line },
  detailFeatureIcon: { width: 40, height: 40, borderRadius: 14, backgroundColor: colors.mint, alignItems: 'center', justifyContent: 'center' },
  detailFeatureText: { flex: 1, color: colors.ink, fontWeight: '800', fontSize: 14 },
  infoCard: { flexDirection: 'row', gap: 10, padding: 14, backgroundColor: '#E5F1F8', borderRadius: radii.md },
  infoText: { flex: 1, color: '#375F76', fontSize: 12, lineHeight: 18 },
  stepDots: { flexDirection: 'row', gap: 6 },
  stepDot: { width: 23, height: 5, borderRadius: 3, backgroundColor: colors.line },
  stepDotActive: { backgroundColor: colors.green },
  stepText: { color: colors.muted, fontSize: 13, fontWeight: '800', width: 44, textAlign: 'right' },
  contributionContent: { padding: 22, paddingBottom: 36, gap: 15, maxWidth: 720, width: '100%', alignSelf: 'center' },
  contributionTitle: { color: colors.ink, fontSize: 30, lineHeight: 35, fontWeight: '900', letterSpacing: -1 },
  contributionSubtitle: { color: colors.muted, fontSize: 15, lineHeight: 22, marginBottom: 8 },
  kindRow: { flexDirection: 'row', gap: 11, marginVertical: 5 },
  selectCard: { flex: 1, minHeight: 178, backgroundColor: colors.surface, borderRadius: radii.lg, borderWidth: 2, borderColor: colors.line, padding: 14, alignItems: 'flex-start' },
  selectCardActive: { borderColor: colors.green, backgroundColor: '#F5FBF8' },
  selectIcon: { width: 43, height: 43, borderRadius: 15, backgroundColor: colors.mint, alignItems: 'center', justifyContent: 'center' },
  selectIconActive: { backgroundColor: colors.green },
  selectTitle: { color: colors.ink, fontSize: 15, lineHeight: 19, fontWeight: '900', marginTop: 12 },
  selectDetail: { color: colors.muted, fontSize: 11, lineHeight: 16, marginTop: 3, marginBottom: 8, flex: 1 },
  inputLabel: { color: colors.ink, fontSize: 14, fontWeight: '900', marginTop: 8 },
  textFieldWrap: { minHeight: 56, backgroundColor: colors.surface, borderRadius: radii.md, borderWidth: 1, borderColor: colors.line, flexDirection: 'row', alignItems: 'center', paddingHorizontal: 15, gap: 9 },
  textField: { flex: 1, paddingVertical: 15, color: colors.ink, fontSize: 16 },
  starRow: { flexDirection: 'row', gap: 13, justifyContent: 'center', backgroundColor: colors.surface, borderRadius: radii.md, paddingVertical: 18, borderWidth: 1, borderColor: colors.line },
  featureGrid: { flexDirection: 'row', flexWrap: 'wrap', gap: 9 },
  optionChip: { minHeight: 47, borderRadius: radii.md, borderWidth: 1, borderColor: colors.line, backgroundColor: colors.surface, paddingHorizontal: 12, flexDirection: 'row', alignItems: 'center', gap: 7, maxWidth: '100%' },
  optionChipActive: { backgroundColor: colors.green, borderColor: colors.green },
  optionText: { color: colors.ink, fontSize: 12, fontWeight: '800' },
  optionTextActive: { color: colors.surface },
  commentField: { minHeight: 150, backgroundColor: colors.surface, borderRadius: radii.md, borderWidth: 1, borderColor: colors.line, padding: 15, color: colors.ink, fontSize: 15, lineHeight: 21 },
  rewardCard: { flexDirection: 'row', alignItems: 'center', backgroundColor: colors.amberSoft, borderRadius: radii.md, padding: 15, gap: 11 },
  rewardIcon: { width: 46, height: 46, borderRadius: 16, backgroundColor: colors.lime, alignItems: 'center', justifyContent: 'center' },
  rewardTitle: { color: colors.ink, fontSize: 17, fontWeight: '900' },
  rewardText: { color: colors.muted, fontSize: 11, marginTop: 2 },
  reviewSummary: { backgroundColor: colors.surface, borderRadius: radii.md, borderWidth: 1, borderColor: colors.line, padding: 15, gap: 11 },
  reviewSummaryTitle: { color: colors.ink, fontWeight: '900', fontSize: 16, marginBottom: 2 },
  summaryRow: { flexDirection: 'row', justifyContent: 'space-between', gap: 15 },
  summaryLabel: { color: colors.muted, fontSize: 13 },
  summaryValue: { color: colors.ink, fontSize: 13, fontWeight: '800', flexShrink: 1, textAlign: 'right' },
  contributionFooter: { paddingHorizontal: 20, paddingTop: 12, paddingBottom: 12, backgroundColor: colors.surface, borderTopWidth: 1, borderTopColor: colors.line, flexDirection: 'row', gap: 10, alignItems: 'center' },
});
