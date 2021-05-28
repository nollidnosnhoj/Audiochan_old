import { HStack, IconButton } from "@chakra-ui/react";
import React, { useCallback, useMemo } from "react";
import {
  MdPause,
  MdPlayArrow,
  MdSkipNext,
  MdSkipPrevious,
} from "react-icons/md";
import { useAudioPlayer } from "~/lib/hooks";

interface PlayerControlsProps {
  size?: "desktop";
}

export default function PlayerControls(props: PlayerControlsProps) {
  const { size = "desktop" } = props;
  const { isPlaying, playIndex, queue, togglePlaying, playPrevious, playNext } =
    useAudioPlayer();

  const buttonSize = useMemo(() => {
    switch (size) {
      default:
        return "md";
    }
  }, [size]);

  const handleTogglePlay = useCallback(() => {
    if (playIndex !== undefined) {
      togglePlaying();
    }
  }, [playIndex]);

  return (
    <HStack>
      <IconButton
        icon={<MdSkipPrevious />}
        aria-label="Previous"
        title="Previous"
        onClick={() => playPrevious()}
        disabled={playIndex === 0 || playIndex === undefined}
        isRound
        variant="ghost"
        size={buttonSize}
        fontSize="25px"
      />
      <IconButton
        icon={isPlaying ? <MdPause /> : <MdPlayArrow />}
        onClick={handleTogglePlay}
        aria-label={isPlaying ? "Pause" : "Play"}
        title={isPlaying ? "Pause" : "Play"}
        isRound
        size={buttonSize}
        colorScheme="primary"
        fontSize="25px"
        disabled={playIndex === undefined}
      />
      <IconButton
        icon={<MdSkipNext />}
        aria-label="Next"
        title="Next"
        onClick={() => playNext()}
        disabled={playIndex === queue.length - 1 || playIndex === undefined}
        isRound
        variant="ghost"
        size={buttonSize}
        fontSize="25px"
      />
    </HStack>
  );
}
