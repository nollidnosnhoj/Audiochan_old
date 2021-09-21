import React from "react";
import { Button } from "@chakra-ui/react";
import { z } from "zod";
import TextInput from "~/components/Forms/Inputs/Text";
import { validationMessages, errorToast } from "~/utils";
import { passwordRule } from "../schemas";
import request from "~/lib/http";
import { useRouter } from "next/router";
import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";

// const updatePasswordSchema: yup.SchemaOf<UpdatePasswordValues> = yup
//   .object()
//   .shape({
//     currentPassword: yup
//       .string()
//       .required(validationMessages.required("Current Password")),
//     newPassword: passwordRule("New Password"),
//     confirmPassword: yup
//       .string()
//       .required()
//       .oneOf([yup.ref("newPassword")], "Password does not match."),
//   });

const updatePasswordSchema = z
  .object({
    currentPassword: z
      .string()
      .min(1, validationMessages.required("Current Password")),
    newPassword: passwordRule("New Password"),
    confirmPassword: z
      .string()
      .min(1, validationMessages.required("Confirm Password")),
  })
  .superRefine((arg, ctx) => {
    if (arg.confirmPassword !== arg.newPassword) {
      ctx.addIssue({
        code: "custom",
        message: "Password does not match.",
      });
    }
  });

type UpdatePasswordValues = z.infer<typeof updatePasswordSchema>;

export default function UpdatePassword() {
  const router = useRouter();
  const {
    register,
    reset,
    formState: { isSubmitting, errors },
    handleSubmit,
  } = useForm<UpdatePasswordValues>({
    resolver: yupResolver(updatePasswordSchema),
  });

  const handlePasswordChange = async (values: UpdatePasswordValues) => {
    const { currentPassword, newPassword } = values;
    try {
      await request({
        method: "patch",
        url: "me/password",
        data: {
          currentPassword: currentPassword,
          newPassword: newPassword,
        },
      });
      reset();
      router.push("/logout");
    } catch (err) {
      errorToast(err);
    }
  };

  return (
    <form onSubmit={handleSubmit(handlePasswordChange)}>
      <TextInput
        {...register("currentPassword")}
        error={errors.currentPassword?.message}
        type="password"
        label="Current Password"
        isRequired
      />
      <TextInput
        {...register("newPassword")}
        type="password"
        error={errors.newPassword?.message}
        label="New Password"
        isRequired
      />
      <TextInput
        {...register("confirmPassword")}
        type="password"
        error={errors.confirmPassword?.message}
        label="Confirm Password"
        isRequired
      />
      <Button
        type="submit"
        isLoading={isSubmitting}
        disabled={isSubmitting}
        loadingText="Submitting..."
      >
        Update Password
      </Button>
    </form>
  );
}
