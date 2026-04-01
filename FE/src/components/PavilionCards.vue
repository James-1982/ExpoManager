<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="pavilion in pavilions"
        :key="pavilion.id"
        :item="pavilion"
      >
        <template #extra>
          <!-- Area e PoweredBy -->
          <div class="extra-field">
            Area: {{ pavilion.area ? pavilion.area : '-' }}
          </div>
          <div class="extra-field">
            Powered By: {{ pavilion.poweredBy ? pavilion.poweredBy : '-' }}
          </div>

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
  imageUrl?: string | null;
  numberOfStands: number;
  area?: string;
  poweredBy?: string;
  lastModify: string;
  modifyBy: string;
  state?: string;
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