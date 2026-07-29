import pillsHero from '../assets/images/pills-hero.jpg'
import pillsCapsules from '../assets/images/pills-capsules.webp'
import pillsAssorted from '../assets/images/pills-assorted.jpg'

const IMAGE_MAP = {
  'pills-hero': pillsHero,
  'pills-capsules': pillsCapsules,
  'pills-assorted': pillsAssorted,
}

export function resolveMedicationImage(key) {
  return IMAGE_MAP[key] || pillsHero
}
