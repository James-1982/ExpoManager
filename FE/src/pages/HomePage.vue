<template>
  <div class="home-container">
    <header class="home-header">
      <div class="user-info">
        <div class="user-box">
          <span>{{ authStore.userEmail }}</span>
          <button class="logout-btn" @click="logout">Logout</button>
        </div>
      </div>
      <h1>Expo</h1>
    </header>

    <div class="tab-card-container">
      <div class="tab-bar">
        <div
          v-for="tab in tabs"
          :key="tab.key"
          :class="['tab', { active: selectedTab === tab.key }]"
          @click="selectedTab = tab.key"
        >
          {{ tab.label }}
        </div>
      </div>

      <div class="cards-wrapper">
        <PavilionCards v-if="selectedTab==='pavilion'" :pavilions="pavilionData" />
        <ExhibitionAreaCards v-if="selectedTab==='sector'" :exhibitions="exhibitionData" />
        <StandCards v-if="selectedTab==='stand'" :stands="standData" />
        <CategoryCards v-if="selectedTab==='category'" :categories="categoryData" />
      </div>

      <div v-if="(selectedTab==='stand' || selectedTab==='category') && !hasData(selectedTab)" class="empty-placeholder">
        Nessun contenuto disponibile
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStore';
import PavilionCards from '../components/PavilionCards.vue';
import ExhibitionAreaCards from '../components/ExhibitionAreaCards.vue';
import StandCards from '../components/StandCards.vue';
import CategoryCards from '../components/CategoryCards.vue';
import api from '../api';

const router = useRouter();
const authStore = useAuthStore();

// Tabs
const selectedTab = ref('pavilion');
const tabs = [
  { key: 'pavilion', label: 'Padiglioni' },
  { key: 'sector', label: 'Settori' },
  { key: 'stand', label: 'Stands' },
  { key: 'category', label: 'Categorie Merciologiche' },
];

// Dati
const pavilionData = ref([]);
const exhibitionData = ref([]);
const standData = ref([]);
const categoryData = ref([]);

// Controllo se tab ha dati
const hasData = (tab: string) => {
  switch(tab) {
    case 'pavilion': return pavilionData.value.length > 0;
    case 'sector': return exhibitionData.value.length > 0;
    case 'stand': return standData.value.length > 0;
    case 'category': return categoryData.value.length > 0;
    default: return false;
  }
};

// Fetch dati per tab (solo se vuoto)
const fetchTabData = async (tab: string) => {
  try {
    if (tab === 'pavilion' && pavilionData.value.length === 0) {
      const res = await api.get('/Pavilion');
      pavilionData.value = res.data;
    }
    if (tab === 'sector' && exhibitionData.value.length === 0) {
      const res = await api.get('/ExhibitionArea');
      exhibitionData.value = res.data;
    }
    if (tab === 'stand' && standData.value.length === 0) {
      const res = await api.get('/Stand');
      standData.value = res.data;
    }
    if (tab === 'category' && categoryData.value.length === 0) {
      const res = await api.get('/Category');
      categoryData.value = res.data;
    }
  } catch (err) {
    console.error('Errore fetch dati tab', err);
  }
};

// Watch su tab
watch(selectedTab, (tab) => fetchTabData(tab), { immediate: true });

// Logout
const logout = async () => {
  try {
    await api.post('/Authentication/logout');
  } catch (err) {
    console.warn('Logout fallito', err);
  } finally {
    authStore.clearAuth();
    router.push('/');
  }
};
</script>

<style src="../styles/components/HomePage.css"></style>