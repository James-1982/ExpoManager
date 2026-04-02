<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="exhibition in exhibitions"
        :key="exhibition.id"
        :item="exhibition"
      >
        <template #extra>
            <div class="extra-row extra-block">
              <!-- Colonna sinistra vuota -->
              <div class="extra-left"></div>

              <!-- Colonna destra con Home Page + switch -->
              <div class="extra-right">
                <span class="extra-label">Home Page</span>
                <label class="switch">
                  <input type="checkbox" v-model="exhibition.highlighted" />
                  <span class="slider round"></span>
                </label>
              </div>
            </div>

            <!-- Nome -->
            <div class="fs-large">{{ exhibition.name }}</div>
            <!-- Descrizione -->
            <div class="fs-medium">{{ exhibition.description }}</div>
            <!-- Type -->
            <div class="fs-small">{{ exhibition.type }}</div>
            <!-- Stand associati -->
            <div class="fs-small">
              Stand associati: {{ exhibition.numberOfStands }}
            </div>

          <!-- Chip stato con icona -->
          <StateChip :state="exhibition.state" />
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
  description?: string;
  imageUrl?: string | null;
  numberOfStands: number;
  lastModify: string;
  modifyBy: string;
  type: string;
  state?: string;
  highlighted:boolean
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