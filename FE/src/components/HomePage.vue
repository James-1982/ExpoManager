<template>
    <div class="home-container">
      <h1>Expo</h1>
  
      <!-- Container unico sempre visibile -->
      <div class="tab-card-container">
        <!-- Tab schede unite centrali -->
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
  
        <!-- Card dinamiche direttamente dentro il container -->
      <div class="cards-wrapper">
        <PavilionCards v-if="selectedTab === 'pavilion'" />
        <ExhibitionAreaCards v-else-if="selectedTab === 'sector'" />
        <StandCards v-else-if="selectedTab === 'stand'" />
        <CategoryCards v-else-if="selectedTab === 'category'" />
      </div>
  
        <!-- Placeholder se nessuna card disponibile -->
        <div v-if="(selectedTab === 'stand' || selectedTab === 'category')" class="empty-placeholder">
          Nessun contenuto disponibile
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref } from 'vue';
  import PavilionCards from './PavilionCards.vue';
  import ExhibitionAreaCards from './ExhibitionAreaCards.vue';
  
  const tabs = [
    { key: 'pavilion', label: 'Padiglioni' },
    { key: 'sector', label: 'Settori' },
    { key: 'stand', label: 'Stands' },
    { key: 'category', label: 'Categorie Merciologiche' },
  ];
  
  const selectedTab = ref('pavilion');
  </script>
  
  <style src="../styles/components/HomePage.css"></style>