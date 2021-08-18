import { useMutation, UseMutationResult, useQueryClient } from "react-query";
import { GET_YOUR_AUDIOS_KEY } from "~/features/auth/api/hooks/useYourAudios";
import { useUser } from "~/features/user/hooks";
import { GET_USER_AUDIOS_QUERY_KEY } from "~/features/user/api/hooks/useGetUserAudios";
import { useAudioQueue } from "~/lib/stores";
import { AudioId } from "../types";
import { GET_AUDIO_QUERY_KEY } from "./useGetAudio";
import { GET_AUDIO_LIST_QUERY_KEY } from "./useGetAudioList";
import request from "~/lib/http";
import { useCallback } from "react";

export function useRemoveAudio(id: AudioId): UseMutationResult<void> {
  const { removeAudioFromQueue } = useAudioQueue();
  const queryClient = useQueryClient();
  const { user } = useUser();
  const removeAudio = useCallback(async () => {
    await request({
      method: "delete",
      url: `audios/${id}`,
    });
  }, [id]);

  return useMutation(removeAudio, {
    async onSuccess() {
      await removeAudioFromQueue(id);
      queryClient.invalidateQueries(GET_AUDIO_LIST_QUERY_KEY);
      queryClient.invalidateQueries(GET_AUDIO_QUERY_KEY(id), { exact: true });
      if (user) {
        queryClient.invalidateQueries(GET_USER_AUDIOS_QUERY_KEY(user.username));
        queryClient.invalidateQueries(GET_YOUR_AUDIOS_KEY);
      }
    },
  });
}
