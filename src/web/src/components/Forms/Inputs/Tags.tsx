import {
  FormControl,
  FormErrorMessage,
  FormLabel,
  Input,
  List,
  ListItem,
  Tag,
  TagCloseButton,
  TagLabel,
  Wrap,
  WrapItem,
} from "@chakra-ui/react";
import React, { useCallback, useMemo, useState } from "react";
import slugify from "slugify";
import { z } from "zod";

interface TagInputProps {
  name: string;
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
  errors?: string[];
  disabled?: boolean;
}

const TagInput: React.FC<TagInputProps> = ({
  name,
  value,
  onChange,
  errors,
  disabled = false,
}) => {
  const [currentInput, setCurrentInput] = useState("");
  const [inputErrors, setInputErrors] = useState(errors);
  const tags = useMemo(() => {
    return value.length === 0 ? [] : value.filter((val) => val.length > 0);
  }, [value]);
  const validationSchema = useMemo(() => {
    return z
      .string()
      .min(3, "Input must have at least 3 characters long.")
      .max(25, "Input must have no more than 25 characters long.")
      .superRefine((args, ctx) => {
        if (value.includes(args)) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "No duplicate tags.",
          });
        }
      });
  }, [value]);

  const applyValidationSchema = useCallback(
    async (input: string): Promise<[boolean, string]> => {
      const result = await validationSchema.safeParseAsync(input);
      if (result.success) return [true, ""];
      return [false, result.error.message];
    },
    [validationSchema]
  );

  const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCurrentInput(e.target.value);
  };

  const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e?.preventDefault();
      onAddTag();
    }
  };

  const onAddTag = async () => {
    const taggifyTag = slugify(currentInput, { lower: true });
    if (validationSchema) {
      const [isValid, errorMessage] = await applyValidationSchema(taggifyTag);
      if (!isValid) {
        setInputErrors([errorMessage]);
        return;
      }
    }
    onChange([...tags, taggifyTag]);
    setCurrentInput("");
    setInputErrors([]);
  };

  const removeTag = (idx: number) => {
    if (idx < 0 || idx >= tags.length) return;
    const filtered = [...tags];
    filtered.splice(idx, 1);
    onChange(filtered);
  };

  return (
    <FormControl paddingY={2} id={name} isInvalid={!!inputErrors}>
      <FormLabel>Tags</FormLabel>
      <Input
        name={name}
        value={currentInput}
        onChange={onInputChange}
        onKeyDown={onKeyDown}
        disabled={disabled}
      />
      <FormErrorMessage>
        <List>
          {inputErrors?.map((err, i) => (
            <ListItem key={i} color="red">
              {err}
            </ListItem>
          ))}
        </List>
      </FormErrorMessage>
      {!!tags.length && (
        <Wrap marginTop={4}>
          {tags.map((tag, idx) => (
            <WrapItem key={idx}>
              <Tag size="md" borderRadius="full" colorScheme="primary">
                <TagLabel>{tag}</TagLabel>
                <TagCloseButton onClick={() => removeTag(idx)} />
              </Tag>
            </WrapItem>
          ))}
        </Wrap>
      )}
    </FormControl>
  );
};

export default TagInput;
