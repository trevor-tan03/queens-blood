export const getTimeFormat = (deadline: Date) => {
  const currentTime = new Date();
  const timeDiffSecs = Math.floor(
    (deadline.getTime() - currentTime.getTime()) / 1000
  );
  return timeDiffSecs;
};
