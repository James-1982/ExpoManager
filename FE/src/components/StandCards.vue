<template>
    <div class="page-container">
      <div class="cards-container" v-if="stands.length">
        <CardBase
          v-for="stand in stands"
          :key="stand.id"
          :item="stand">
            <template #extra>
              <!-- Nome -->
              <div class="fs-large">{{ stand.name }}</div>
              <!-- Descrizione -->
              <div class="fs-medium">{{ stand.description }}</div>
              <!-- Pavilion -->
              <div class="fs-small">Padiglione: {{ stand.pavilionName }}</div>
              <!-- Sector -->
              <div class="fs-small">Settore: {{ stand.exhibitionAreaName }}</div>
              <!-- Dimensions -->
              <div class="fs-small">Dimension: {{ stand.width }}x{{ stand.length }}</div>
          </template>
        </CardBase>
      </div>
      <div v-else class="empty-placeholder">Nessun contenuto disponibile</div>
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import api from '../api';
  import CardBase from './CardBase.vue';
  
  interface Stand {
    id: number;
    name: string;
    description?: string;
    pavilionName?: string;
    exhibitionAreaName?: string;
    imageUrl?: string | null;
    lastModify: string;
    modifyBy: string;
    state?: string;
    width: number;
    length: number;
  }
  
  const stands = ref<Stand[]>([]);
  
  onMounted(async () => {
    try {
      const res = await api.get('/Stand');
      stands.value = res.data.map((s: any) => ({
        ...s,
        lastModify: s.lastModify || '',
        modifyBy: s.modifyBy || ''
      }));
    } catch (err) {
      console.error(err);
    }
  });
  </script>
  
  <style src="../styles/components/StandCard.css"></style>