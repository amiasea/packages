package main

import "github.com/amiasea/packages/terraforge-cli/cmd"

func main() {
    cmd.Execute()
}

// // Magnum: a quiet, irreversible decision

// type Intent = {
//   caller:  "human";
//   mode:    "present-tense";
//   crutch:  "none";
// };

// type Field<TIntent extends Intent> = {
//   enteredAt:  Date;
//   vector:     "one-way";
//   status:     "in-progress" | "complete";
// };

// declare function magnum<TIntent extends Intent>(
//   intent: TIntent
// ): Field<TIntent>;