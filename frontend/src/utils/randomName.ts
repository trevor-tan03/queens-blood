/*
    Give a random name to a player if they did not create their own
 */
const GetRandomName = () => {
  const avalancheMembers = [
    "Cloud",
    "Barret",
    "Tifa",
    "Aerith",
    "Caith Sith",
    "Cid",
    "Vincent",
    "Red XIII",
    "Yuffie",
  ];
  const randomIndex = Math.floor(Math.random() * avalancheMembers.length);
  return avalancheMembers[randomIndex];
};

export default GetRandomName;
