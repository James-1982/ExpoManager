<template>
  <div class="home-container">
    <header class="home-header">
<div class="user-info">
  <div class="user-box">
    <span>{{ userEmail }}</span>
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
        <!-- Passa i dati tramite props alle card -->
        <PavilionCards v-show="selectedTab==='pavilion'" :pavilions="pavilionData"/>
        <ExhibitionAreaCards v-show="selectedTab==='sector'" :exhibitions="exhibitionData"/>
        <StandCards v-show="selectedTab==='stand'" :stands="standData"/>
        <CategoryCards v-show="selectedTab==='category'" :categories="categoryData"/>
      </div>

      <div v-if="selectedTab==='stand' || selectedTab==='category'" class="empty-placeholder">
        Nessun contenuto disponibile
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import PavilionCards from './PavilionCards.vue';
import ExhibitionAreaCards from './ExhibitionAreaCards.vue';
import StandCards from './StandCards.vue';
import CategoryCards from './CategoryCards.vue';
import api from '../api';

const router = useRouter();
const selectedTab = ref('pavilion');

const tabs = [
  { key: 'pavilion', label: 'Padiglioni' },
  { key: 'sector', label: 'Settori' },
  { key: 'stand', label: 'Stands' },
  { key: 'category', label: 'Categorie Merciologiche' },
];

const pavilionData = ref([]);
const exhibitionData = ref([]);
const standData = ref([]);
const categoryData = ref([]);

const userEmail = ref(localStorage.getItem('user_email') || '');

// 🔥 Lazy load per tab
watch(selectedTab, async (tab) => {
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
    console.error(err);
  }
}, { immediate: true }); // carica subito il primo tab

const logout = async () => {
  try { await api.post('/Authentication/logout'); } 
  catch (err) { console.warn('Logout fallito', err); } 
  finally {
    localStorage.clear();
    sessionStorage.clear();
    router.push('/');
  }
};
</script>

<style src="../styles/components/HomePage.css"></style>