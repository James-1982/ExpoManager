<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="pavilion in pavilions"
        :key="pavilion.id"
        :item="pavilion"
      >
        <template #extra>
          <div class="fs-large">{{ pavilion.name }}</div>
          <div class="fs-medium">{{ pavilion.description }}</div>
          <div class="fs-small">Area: {{ pavilion.area }}</div>
          <div class="fs-small">Powered By: {{ pavilion.poweredBy }}</div>
        </template>
      </CardBase>
    </div>

    <div v-if="pavilions.length === 0" class="empty-placeholder">
      Nessun padiglione disponibile
    </div>
  </div>
</template>

<script lang="ts" setup>
import CardBase from './CardBase.vue';

interface Pavilion {
  id: number;
  name: string;
  description?: string;
  area?: string;
  poweredBy?: string;
  imageUrl?: string | null;
  lastModify: string;
  modifyBy: string;
}

const props = defineProps<{ pavilions: Pavilion[] }>();
</script>

<style src="../styles/components/PavilionCard.css"></style>