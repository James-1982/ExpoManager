<template>
  <div class="page-container">
    <div class="cards-container" v-if="stands.length">
      <CardBase
        v-for="stand in stands"
        :key="stand.id"
        :item="stand"
      >
        <template #extra>
          <div class="fs-large">{{ stand.name }}</div>
          <div class="fs-medium">{{ stand.description }}</div>
          <div class="fs-small">Padiglione: {{ stand.pavilionName }}</div>
          <div class="fs-small">Settore: {{ stand.exhibitionAreaName }}</div>
          <div class="fs-small">Dimension: {{ stand.width }}x{{ stand.length }}</div>
        </template>
      </CardBase>
    </div>

    <div v-else class="empty-placeholder">
      Nessun contenuto disponibile
    </div>
  </div>
</template>

<script lang="ts" setup>
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

// Riceve i dati come prop da HomePage
const props = defineProps<{ stands: Stand[] }>();
</script>

<style src="../styles/components/StandCard.css"></style>