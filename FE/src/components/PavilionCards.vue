<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="pavilion in pavilions"
        :key="pavilion.id"
        :item="pavilion">
        
        <template #extra>
          <!-- Nome -->
          <div class="fs-large">{{ pavilion.name }}</div>
          <!-- Descrizione -->
          <div class="fs-medium">{{ pavilion.description }}</div>
          <!-- Area -->
          <div class="fs-small">Area: {{ pavilion.area }}</div>
          <!-- PoweredBy -->
          <div class="fs-small">PoweredBy: {{ pavilion.poweredBy }}</div>
        </template>

      </CardBase>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import api from '../api';
import CardBase from './CardBase.vue';

// Definizione tipi
interface Pavilion {
  id: number;
  name: string;
  description?:string;
  area?: string;
  poweredBy?: string;
  imageUrl?: string | null;
  lastModify: string;
  modifyBy: string;
}

// Lista dei pavilion
const pavilions = ref<Pavilion[]>([]);

// Caricamento dati dall'API
onMounted(async () => {
  try {
    const res = await api.get('/Pavilion');
    console.log("DATA:", res.data);
    pavilions.value = res.data;
  } catch (err) {
    console.error("ERROR:", err);
  }
});

</script>

<style src="../styles/components/PavilionCard.css"></style>