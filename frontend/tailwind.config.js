export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        
        ink: {
          950: '#060B17',
          900: '#0A1628',
          800: '#0F1F38',
          700: '#16294A',
          600: '#1D3660',
        },
        
        vital: {
          400: '#4DE8DC',
          500: '#14C7B8',
          600: '#0EA89B',
          700: '#0B8A80',
        },
        
        pulse: {
          400: '#F17F73',
          500: '#E85D50',
          600: '#D2453A',
        },
        
        mist: {
          50: '#F6F8FB',
          100: '#EEF2F7',
          200: '#E2E8F0',
          300: '#CBD5E1',
        },
        slate: {
          500: '#64748B',
          600: '#475569',
          700: '#334155',
          800: '#1E293B',
          900: '#0F172A',
        },
      },
      fontFamily: {
        display: ['"Sora"', 'system-ui', 'sans-serif'],
        body: ['"Inter"', 'system-ui', 'sans-serif'],
      },
      boxShadow: {
        glow: '0 0 0 1px rgba(77,232,220,0.15), 0 8px 40px -8px rgba(20,199,184,0.35)',
        card: '0 1px 2px rgba(15,23,42,0.04), 0 8px 24px -8px rgba(15,23,42,0.10)',
        panel: '0 20px 60px -15px rgba(6,11,23,0.45)',
      },
      backgroundImage: {
        'grid-lines':
          'linear-gradient(rgba(77,232,220,0.06) 1px, transparent 1px), linear-gradient(90deg, rgba(77,232,220,0.06) 1px, transparent 1px)',
      },
      keyframes: {
        pulseLine: {
          '0%, 100%': { strokeDashoffset: '0' },
          '50%': { strokeDashoffset: '-24' },
        },
        floatSlow: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        fadeUp: {
          '0%': { opacity: '0', transform: 'translateY(12px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
      },
      animation: {
        pulseLine: 'pulseLine 2.4s linear infinite',
        floatSlow: 'floatSlow 6s ease-in-out infinite',
        fadeUp: 'fadeUp 0.5s ease-out both',
      },
      borderRadius: {
        xl2: '1.25rem',
      },
    },
  },
  plugins: [],
}
