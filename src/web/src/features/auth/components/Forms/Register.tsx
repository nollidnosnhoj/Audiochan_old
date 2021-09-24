import React from "react";
import { Button, Flex, Text } from "@chakra-ui/react";
import { z } from "zod";
import TextInput from "~/components/Forms/Inputs/Text";
import { usernameRule, passwordRule } from "~/features/user/schemas";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

export const registrationValidationSchema = z
  .object({
    username: usernameRule("Username"),
    password: passwordRule("Password"),
    email: z.string().min(1).email(),
    confirmPassword: z.string().min(1),
  })
  .superRefine((arg, ctx) => {
    if (arg.confirmPassword !== arg.password) {
      ctx.addIssue({
        code: "custom",
        message: "Password does not match.",
      });
    }
  });

export type RegisterFormInputs = z.infer<typeof registrationValidationSchema>;

interface RegisterFormProps {
  initialRef?: React.RefObject<HTMLInputElement>;
  onSubmit?: (values: RegisterFormInputs) => Promise<void>;
}

export default function RegisterForm(props: RegisterFormProps) {
  const formik = useForm<RegisterFormInputs>({
    resolver: zodResolver(registrationValidationSchema),
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = formik;

  const handleRegisterSubmit = async (values: RegisterFormInputs) => {
    try {
      await props.onSubmit?.(values);
    } catch {
      reset({ ...values });
    }
  };

  return (
    <form onSubmit={handleSubmit(handleRegisterSubmit)}>
      <TextInput
        {...register("username")}
        ref={props.initialRef}
        error={errors.username?.message}
        label="Username"
        isRequired
      />
      <TextInput
        {...register("email")}
        error={errors.email?.message}
        label="Email"
        isRequired
      />
      <TextInput
        type="password"
        error={errors.password?.message}
        label="Password"
        isRequired
        {...register("password")}
      />
      <TextInput
        type="password"
        error={errors.confirmPassword?.message}
        label="Confirm Password"
        isRequired
        {...register("confirmPassword")}
      />
      <Text fontSize="sm">
        By registering, you agree to our terms and service.
      </Text>
      <Flex justify="flex-end">
        <Button
          marginTop={4}
          width="100%"
          type="submit"
          isLoading={isSubmitting}
          colorScheme="primary"
        >
          Register
        </Button>
      </Flex>
    </form>
  );
}
