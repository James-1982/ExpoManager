<template>
  <div class="auth-page">
    <div class="auth-container">
      <h1>Reset Password</h1>

      <div class="form-group">
        <label for="newPassword">Nuova Password</label>
        <input
          id="newPassword"
          v-model="newPassword"
          type="password"
          placeholder="Inserisci nuova password"
        />
      </div>

      <div class="form-group">
        <label for="confirmPassword">Conferma Password</label>
        <input
          id="confirmPassword"
          v-model="confirmPassword"
          type="password"
          placeholder="Conferma nuova password"
        />
      </div>

      <!-- Messaggio di errore se le password non coincidono -->
      <p class="message error" v-if="newPassword && confirmPassword && !passwordsMatch">
        Le password non coincidono.
      </p>

      <button
        class="btn-auth"
        @click="resetPassword"
        :disabled="loading || !passwordsMatch || !newPassword || !confirmPassword"
      >
        {{ loading ? 'Reset in corso...' : 'Reset Password' }}
      </button>

      <p class="message" v-if="message">{{ message }}</p>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '../api';

const route = useRoute();
const router = useRouter();

const token = ref('');
const newPassword = ref('');
const confirmPassword = ref('');
const loading = ref(false);
const message = ref('');

// Computed per verificare se le password coincidono
const passwordsMatch = computed(() => newPassword.value === confirmPassword.value);

onMounted(() => {
  token.value = (route.query.token as string) || '';
  if (!token.value) {
    message.value = 'Token mancante nel link.';
  }
});

const resetPassword = async () => {
  if (!passwordsMatch.value) {
    message.value = 'Le password non coincidono.';
    return;
  }

  if (!token.value) {
    message.value = 'Token mancante.';
    return;
  }

  loading.value = true;
  message.value = '';

  try {
    await api.post('/Authentication/reset-password', {
      token: token.value,
      newPassword: newPassword.value,
      email: route.query.email, // opzionale se server richiede email
    });

    message.value = 'Password resettata con successo!';
    setTimeout(() => router.push('/'), 1000);
  } catch (err: any) {
    console.error(err);
    message.value = err.response?.data?.message || 'Errore durante il reset.';
  } finally {
    loading.value = false;
  }
};
</script>

<style src="../styles/components/Auth.css"></style>