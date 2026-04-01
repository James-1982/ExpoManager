<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="exhibition in exhibitions"
        :key="exhibition.id"
        :item="exhibition"
      >
        <template #extra>
          <!-- Stand associati -->
          <div class="stand-info">
            Stand associati: {{ exhibition.numberOfStands }}
          </div>

          <!-- Chip stato con icona -->
          <StateChip state="Undefined" /> 
        </template>
      </CardBase>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import api from '../api';
import CardBase from './CardBase.vue';
import StateChip from './StateChip.vue'

// Definizione tipi
interface Exhibition {
  id: number;
  name: string;
  imageUrl?: string | null;
  numberOfStands: number;
  lastModify: string;
  modifyBy: string;
  state?: string;
}

// Lista delle exhibitions
const exhibitions = ref<Exhibition[]>([]);

// Caricamento dati dall'API
onMounted(async () => {
  try {
    const res = await api.get('/ExhibitionArea');
    exhibitions.value = res.data;
  } catch (err) {
    console.error(err);
  }
});
</script>

<!-- CSS separato -->
<style src="../styles/components/ExhibitionAreaCard.css"></style>